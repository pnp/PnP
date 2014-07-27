using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Workflow.CallCustomServiceWeb.Services
{
    public class PartSuppliersService
    {
        private ClientContext clientContext;

        private List list;

        public PartSuppliersService(ClientContext clientContext)
        {
            this.clientContext = clientContext;
            this.list = clientContext.Web.Lists.GetByTitle("Part Suppliers");

            clientContext.Load(list, l => l.Id);
            clientContext.ExecuteQuery();
        }

        public ListItem GetItem(int id)
        {
            try
            {
                var item = list.GetItemById(id);
                clientContext.Load(item);
                clientContext.ExecuteQuery();
                return item;
            }
            catch
            {
                return null;
            }
        }

        public int Add(string country)
        {
            var item = list.AddItem(new ListItemCreationInformation());
            item["Country"] = country;
            item.Update();
            clientContext.ExecuteQuery();
            return item.Id;
        }

        public int? GetIdByCountry(string country)
        {
            string camlStringFormat = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Country' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                        </Query>
                </View>";
            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(camlStringFormat, country);

            var items = list.GetItems(query);
            clientContext.Load(items);
            clientContext.ExecuteQuery();

            if (items.Count == 0) return null;
            return items[0].Id;
        }
        
        public void UpdateSuppliers(string country, string[] supplierNames)
        {
            string camlStringFormat = @"
                <View>
                    <Query>                
                        <Where>
                            <Eq>
                                <FieldRef Name='Country' />
                                <Value Type='Text'>{0}</Value>
                            </Eq>
                        </Where>
                        </Query>
                </View>";
            CamlQuery query = new CamlQuery();
            query.ViewXml = string.Format(camlStringFormat, country);

            var items = list.GetItems(query);
            clientContext.Load(items);
            clientContext.ExecuteQuery();

            if (items.Count > 0)
            {
                var item = items[0];
                string commaSeparatedList = String.Join(",", supplierNames);
                item["Suppliers"] = commaSeparatedList;
                item.Update();
                clientContext.ExecuteQuery();
            }
        }

        #region Workflow

        public WorkflowSubscription GetWorkflowSubscription(string workflowName)
        {
            var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

            // find Approve Suppliers workflow definition
            var deploymentService = workflowServicesManager.GetWorkflowDeploymentService();
            var definitions = deploymentService.EnumerateDefinitions(true);
            clientContext.Load(definitions);
            clientContext.ExecuteQuery();

            var definition = definitions
                .Where(d => d.DisplayName == "Approve Suppliers")
                .First();

            // find subscriptions
            var subscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var subscriptions = subscriptionService.EnumerateSubscriptionsByDefinition(definition.Id);
            clientContext.Load(subscriptions);
            clientContext.ExecuteQuery();

            return subscriptions
                .Where(s => s.EventSourceId == list.Id)
                .First();
        }

        public WorkflowInstance GetItemWorkflowInstance(Guid subscriptionId, int itemId)
        {
            var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

            var instanceService = workflowServicesManager.GetWorkflowInstanceService();
            var instances = instanceService.EnumerateInstancesForListItem(list.Id, itemId);
            clientContext.Load(instances);
            clientContext.ExecuteQuery();

            return instances
                .Where(i => i.WorkflowSubscriptionId == subscriptionId)
                .FirstOrDefault();
        }

        public WorkflowInstance GetWorkflowInstance(Guid instanceId)
        {
            var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);
            var instanceService = workflowServicesManager.GetWorkflowInstanceService();
            var instance = instanceService.GetInstance(instanceId);
            clientContext.Load(instance);
            clientContext.ExecuteQuery();
            return instance;
        }

        public void StartWorkflow(Guid subscriptionId, int itemId, Dictionary<string, object> payload)
        {
            var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

            var subscriptionService = workflowServicesManager.GetWorkflowSubscriptionService();
            var subscription = subscriptionService.GetSubscription(subscriptionId);

            var instanceService = workflowServicesManager.GetWorkflowInstanceService();
            instanceService.StartWorkflowOnListItem(subscription, itemId, payload);
            clientContext.ExecuteQuery();
        }

        public void PublishCustomEvent(Guid instanceId, string eventName, string payload)
        {
            var workflowServicesManager = new WorkflowServicesManager(clientContext, clientContext.Web);

            var instanceService = workflowServicesManager.GetWorkflowInstanceService();
            var instance = instanceService.GetInstance(instanceId);

            instanceService.PublishCustomEvent(instance, eventName, payload);
            clientContext.ExecuteQuery();
        }


        public ListItem GetApprovalTaskForCurrentUser(int itemId)
        {
            var web = clientContext.Web;
            clientContext.Load(web, i => i.Id);
            clientContext.ExecuteQuery();            

            CamlQuery query = new CamlQuery();
            query.ViewXml = @"
                <View>                                            
                    <ViewFields>
                        <FieldRef Name='Id'/>
                        <FieldRef Name='RelatedItems'/>
                        <FieldRef Name='Status' />
                    </ViewFields>
                    <Query>                          
                        <Where>
                            <Eq>
                                <FieldRef Name='AssignedTo' />
                                <Value Type='Integer'>
                                    <UserID />
                                </Value>
                            </Eq>
                        </Where>
                    </Query>
                </View>";

            var taskList = web.Lists.GetByTitle("WorkflowTaskList");
            var tasks = taskList.GetItems(query);
            clientContext.Load(tasks);
            clientContext.ExecuteQuery();

            if (tasks.Count == 0) return null;

            var relatedItems = string.Format(
                "[{{\"ItemId\":{0},\"WebId\":\"{1:d}\",\"ListId\":\"{2:d}\"}}]",
                itemId, web.Id, list.Id);

            return tasks
                .Where(t => ((string)t["RelatedItems"]) == relatedItems)
                .FirstOrDefault();
        }

        #endregion
    }
}