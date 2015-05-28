using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.SharePoint.Client.WorkflowServices;

namespace Microsoft.SharePoint.Client
{
    public static partial class WorkflowExtensions
    {
        #region Subscriptions
        /// <summary>
        /// Returns a workflow subscription for a site.
        /// </summary>
        /// <param name="web"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static WorkflowSubscription GetWorkflowSubscription(this Web web, string name)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscriptions = subscriptionService.EnumerateSubscriptions();
            var subscriptionQuery = from sub in subscriptions where sub.Name == name select sub;
            var subscriptionsResults = web.Context.LoadQuery(subscriptionQuery);
            web.Context.ExecuteQueryRetry();
            var subscription = subscriptionsResults.FirstOrDefault();
            return subscription;

        }

        /// <summary>
        /// Returns a workflow subscription
        /// </summary>
        /// <param name="web"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkflowSubscription GetWorkflowSubscription(this Web web, Guid id)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscription = subscriptionService.GetSubscription(id);
            web.Context.Load(subscription);
            web.Context.ExecuteQueryRetry();
            return subscription;
        }

        /// <summary>
        /// Returns a workflow subscription (associations) for a list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="name"></param>
        /// <returns></returns>
        public static WorkflowSubscription GetWorkflowSubscription(this List list, string name)
        {
            var servicesManager = new WorkflowServicesManager(list.Context, list.ParentWeb);
            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
            var subscriptions = subscriptionService.EnumerateSubscriptionsByList(list.Id);
            var subscriptionQuery = from sub in subscriptions where sub.Name == name select sub;
            var subscriptionResults = list.Context.LoadQuery(subscriptionQuery);
            list.Context.ExecuteQueryRetry();
            var subscription = subscriptionResults.FirstOrDefault();
            return subscription;
        }

        /// <summary>
        /// Adds a workflow subscription
        /// </summary>
        /// <param name="list"></param>
        /// <param name="workflowDefinitionName">The name of the workflow definition <seealso>
        ///         <cref>WorkflowExtensions.GetWorkflowDefinition</cref>
        ///     </seealso>
        /// </param>
        /// <param name="subscriptionName">The name of the workflow subscription to create</param>
        /// <param name="startManually">if True the workflow can be started manually</param>
        /// <param name="startOnCreate">if True the workflow will be started on item creation</param>
        /// <param name="startOnChange">if True the workflow will be started on item change</param>
        /// <param name="historyListName">the name of the history list. If not available it will be created</param>
        /// <param name="taskListName">the name of the task list. If not available it will be created</param>
        /// <param name="associationValues"></param>
        /// <returns>Guid of the workflow subscription</returns>
        public static Guid AddWorkflowSubscription(this List list, string workflowDefinitionName, string subscriptionName, bool startManually, bool startOnCreate, bool startOnChange, string historyListName, string taskListName, Dictionary<string, string> associationValues = null)
        {
            var definition = list.ParentWeb.GetWorkflowDefinition(workflowDefinitionName, true);

            return AddWorkflowSubscription(list, definition, subscriptionName, startManually, startOnCreate, startOnChange, historyListName, taskListName, associationValues);
        }

        /// <summary>
        /// Adds a workflow subscription to a list
        /// </summary>
        /// <param name="list"></param>
        /// <param name="workflowDefinition">The workflow definition. <seealso>
        ///         <cref>WorkflowExtensions.GetWorkflowDefinition</cref>
        ///     </seealso>
        /// </param>
        /// <param name="subscriptionName">The name of the workflow subscription to create</param>
        /// <param name="startManually">if True the workflow can be started manually</param>
        /// <param name="startOnCreate">if True the workflow will be started on item creation</param>
        /// <param name="startOnChange">if True the workflow will be started on item change</param>
        /// <param name="historyListName">the name of the history list. If not available it will be created</param>
        /// <param name="taskListName">the name of the task list. If not available it will be created</param>
        /// <param name="associationValues"></param>
        /// <returns>Guid of the workflow subscription</returns>
        public static Guid AddWorkflowSubscription(this List list, WorkflowDefinition workflowDefinition, string subscriptionName, bool startManually, bool startOnCreate, bool startOnChange, string historyListName, string taskListName, Dictionary<string, string> associationValues = null)
        {
            // parameter validation
            subscriptionName.ValidateNotNullOrEmpty("subscriptionName");
            historyListName.ValidateNotNullOrEmpty("historyListName");
            taskListName.ValidateNotNullOrEmpty("taskListName");

            var historyList = list.ParentWeb.GetListByTitle(historyListName);
            if (historyList == null)
            {
                historyList = list.ParentWeb.CreateList(ListTemplateType.WorkflowHistory, historyListName, false);
            }
            var taskList = list.ParentWeb.GetListByTitle(taskListName);
            if (taskList == null)
            {
                taskList = list.ParentWeb.CreateList(ListTemplateType.Tasks, taskListName, false);
            }


            var sub = new WorkflowSubscription(list.Context);

            sub.DefinitionId = workflowDefinition.Id;
            sub.Enabled = true;
            sub.Name = subscriptionName;

            var eventTypes = new List<string>();
            if (startManually) eventTypes.Add("WorkflowStart");
            if (startOnCreate) eventTypes.Add("ItemAdded");
            if (startOnChange) eventTypes.Add("ItemUpdated");

            sub.EventTypes = eventTypes;

            sub.SetProperty("HistoryListId", historyList.Id.ToString());
            sub.SetProperty("TaskListId", taskList.Id.ToString());

            if (associationValues != null)
            {
                foreach (var key in associationValues.Keys)
                {
                    sub.SetProperty(key, associationValues[key]);
                }
            }

            var servicesManager = new WorkflowServicesManager(list.Context, list.ParentWeb);

            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();

            var subscriptionResult = subscriptionService.PublishSubscriptionForList(sub, list.Id);

            list.Context.ExecuteQueryRetry();

            return subscriptionResult.Value;
        }

      

        /// <summary>
        /// Deletes the subscription
        /// </summary>
        /// <param name="subscription"></param>
        public static void Delete(this WorkflowSubscription subscription)
        {
            var clientContext = subscription.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

            var subscriptionService = servicesManager.GetWorkflowSubscriptionService();

            subscriptionService.DeleteSubscription(subscription.Id);

            clientContext.ExecuteQueryRetry();
        }
        #endregion

        #region Definitions
        /// <summary>
        /// Returns a workflow definition for a site
        /// </summary>
        /// <param name="web"></param>
        /// <param name="displayName"></param>
        /// <param name="publishedOnly"></param>
        /// <returns></returns>
        public static WorkflowDefinition GetWorkflowDefinition(this Web web, string displayName, bool publishedOnly = true)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var deploymentService = servicesManager.GetWorkflowDeploymentService();
            var definitions = deploymentService.EnumerateDefinitions(publishedOnly);
            var definitionQuery = from def in definitions where def.DisplayName == displayName select def;
            var definitionResults = web.Context.LoadQuery(definitionQuery);
            web.Context.ExecuteQueryRetry();
            var definition = definitionResults.FirstOrDefault();
            return definition;
        }

        /// <summary>
        /// Returns a workflow definition
        /// </summary>
        /// <param name="web"></param>
        /// <param name="id"></param>
        /// <returns></returns>
        public static WorkflowDefinition GetWorkflowDefinition(this Web web, Guid id)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var deploymentService = servicesManager.GetWorkflowDeploymentService();
            var definition = deploymentService.GetDefinition(id);
            web.Context.Load(definition);
            web.Context.ExecuteQueryRetry();
            return definition;
        }

        /// <summary>
        /// Deletes a workflow definition
        /// </summary>
        /// <param name="definition"></param>
        public static void Delete(this WorkflowDefinition definition)
        {
            var clientContext = definition.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var deploymentService = servicesManager.GetWorkflowDeploymentService();
            deploymentService.DeleteDefinition(definition.Id);
            clientContext.ExecuteQueryRetry();
        }
        #endregion

        #region Instances
        /// <summary>
        /// Returns alls workflow instances for a site
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static WorkflowInstanceCollection GetWorkflowInstances(this Web web)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            var instances = workflowInstanceService.EnumerateInstancesForSite();
            web.Context.Load(instances);
            web.Context.ExecuteQueryRetry();
            return instances;
        }

        /// <summary>
        /// Returns alls workflow instances for a list item
        /// </summary>
        /// <param name="web"></param>
        /// <param name="item"></param>
        /// <returns></returns>
        public static WorkflowInstanceCollection GetWorkflowInstances(this Web web, ListItem item)
        {
            var servicesManager = new WorkflowServicesManager(web.Context, web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            var instances = workflowInstanceService.EnumerateInstancesForListItem(item.ParentList.Id, item.Id);
            web.Context.Load(instances);
            web.Context.ExecuteQueryRetry();
            return instances;
        }

        /// <summary>
        /// Returns all instances of a workflow for this subscription
        /// </summary>
        /// <param name="subscription"></param>
        /// <returns></returns>
        public static WorkflowInstanceCollection GetInstances(this WorkflowSubscription subscription)
        {
            var clientContext = subscription.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            var instances = workflowInstanceService.Enumerate(subscription);
            clientContext.Load(instances);
            clientContext.ExecuteQueryRetry();
            return instances;
        }

        /// <summary>
        /// Cancels a workflow instance
        /// </summary>
        /// <param name="instance"></param>
        public static void CancelWorkFlow(this WorkflowInstance instance)
        {
            var clientContext = instance.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            workflowInstanceService.CancelWorkflow(instance);
            clientContext.ExecuteQueryRetry();
        }

        /// <summary>
        /// Resumes a workflow
        /// </summary>
        /// <param name="instance"></param>
        public static void ResumeWorkflow(this WorkflowInstance instance)
        {
            var clientContext = instance.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            workflowInstanceService.ResumeWorkflow(instance);
            clientContext.ExecuteQueryRetry();
        }
        #endregion

        #region Messaging

        /// <summary>
        /// Publish a custom event to a target workflow instance
        /// </summary>
        /// <param name="instance"></param>
        /// <param name="eventName">The name of the target event</param>
        /// <param name="payload">The payload that will be sent to the event</param>
        public static void PublishCustomEvent(this WorkflowInstance instance, String eventName, String payload)
        {
            var clientContext = instance.Context as ClientContext;
            var servicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var workflowInstanceService = servicesManager.GetWorkflowInstanceService();
            workflowInstanceService.PublishCustomEvent(instance, eventName, payload);
            clientContext.ExecuteQueryRetry();
        }

        #endregion
    }
}
