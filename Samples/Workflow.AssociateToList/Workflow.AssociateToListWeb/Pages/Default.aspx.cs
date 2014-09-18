using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace Workflow.AssociateToListWeb {
    public partial class Default : System.Web.UI.Page {
        const string WF_HISTORY_LIST_TITLE = "WorkflowHistoryList";
        const string WF_TASK_LIST_TITLE = "WorkflowTaskList";
        const string WORKFLOW_TASK_CTYPE = "0x0108003365C4474CAE8C42BCE396314E88E51F";
        static readonly Guid WorkflowDefinitionId = new Guid("{87333FF5-BFF4-4645-8EB8-C3BA0B2FFE6E}");
        static readonly Guid WorkflowSubscriptionId = new Guid("{84F0C5F6-B684-413F-A289-46C5C2BEE5EF}");

        protected void Page_PreInit(object sender, EventArgs e) {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl)) {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e) {
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                Response.Write(clientContext.Web.Title);
            }
        }

        protected void DeployWorkflowButton_Click(object sender, EventArgs e) {

            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost()) {
                var targetListForWorkflow = clientContext.Web.Lists.GetByTitle("Event Registration");
                clientContext.Load(clientContext.Web);
                clientContext.Load(targetListForWorkflow);
                clientContext.ExecuteQuery();

                // ensure the workflow history and task lists are created
                EnsureWorkflowHistoryAndTaskLists(clientContext);

                var xamlFileAsString = System.IO.File.ReadAllText(Server.MapPath("/Workflow/Workflow.xaml"));
                var wfSvcMgr = new WorkflowServicesManager(clientContext, clientContext.Web);
                var deploySvc = wfSvcMgr.GetWorkflowDeploymentService();
                var subscriptionSvc = wfSvcMgr.GetWorkflowSubscriptionService();

                var wfDefinition = new WorkflowDefinition(clientContext) {
                    Id = WorkflowDefinitionId,
                    Xaml = xamlFileAsString,
                    DisplayName = "Test Workflow"
                };

                var wfDefinitionResult = deploySvc.SaveDefinition(wfDefinition);
                clientContext.ExecuteQuery();

                deploySvc.PublishDefinition(WorkflowDefinitionId);
                clientContext.ExecuteQuery();

                var wfSubscription = new WorkflowSubscription(clientContext) {
                    DefinitionId = WorkflowDefinitionId,
                    Name = "Test Workflow",
                    Id = WorkflowSubscriptionId,
                    Enabled = true
                };
                var wfSubscriptionResult = subscriptionSvc.PublishSubscriptionForList(wfSubscription, targetListForWorkflow.Id);
                clientContext.ExecuteQuery();
            }
        }

        private void EnsureWorkflowHistoryAndTaskLists(ClientContext clientContext) {
            var lists = clientContext.Web.Lists;
            clientContext.Load(lists, ls => ls.Where(l => l.Title == WF_HISTORY_LIST_TITLE || l.Title == WF_TASK_LIST_TITLE));
            clientContext.ExecuteQuery();

            if (lists.FirstOrDefault(l => l.Title == WF_HISTORY_LIST_TITLE) == null) {
                CreateList(lists, WF_HISTORY_LIST_TITLE, ListTemplateType.WorkflowHistory);
            }
            if (lists.FirstOrDefault(l => l.Title == WF_TASK_LIST_TITLE) == null) {
                var taskList = CreateList(lists, WF_TASK_LIST_TITLE, ListTemplateType.TasksWithTimelineAndHierarchy);
                var ctype = clientContext.Web.ContentTypes.GetById(WORKFLOW_TASK_CTYPE);
                clientContext.Load(ctype);
                clientContext.ExecuteQuery();

                taskList.ContentTypes.AddExistingContentType(ctype);
                clientContext.ExecuteQuery();
            }
        }

        private List CreateList(ListCollection lists, string listTitle, ListTemplateType listTemplateType) {
            var listCreationInfo = new ListCreationInformation() {
                Title = listTitle,
                TemplateType = (int)listTemplateType,
                Url = "Lists/"+listTitle
            };
            var list = lists.Add(listCreationInfo);
            lists.Context.Load(list);
            lists.Context.ExecuteQuery();
            return list;
        }
    }
}