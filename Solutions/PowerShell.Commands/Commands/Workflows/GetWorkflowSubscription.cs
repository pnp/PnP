using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Get, "SPOWorkflowSubscription")]
    [CmdletHelp("Returns a workflow subscriptions from a list", Category = "Workflows")]

    public class GetWorkflowSubscription : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The name of the workflow", Position = 0)]
        public string Name;

        [Parameter(Mandatory = false, HelpMessage = "A list to search the association for", Position = 1)]
        public ListPipeBind List;
        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = SelectedWeb.GetList(List);

                if (string.IsNullOrEmpty(Name))
                {
                    var servicesManager = new WorkflowServicesManager(ClientContext, SelectedWeb);
                    var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
                    var subscriptions = subscriptionService.EnumerateSubscriptionsByList(list.Id);

                    ClientContext.Load(subscriptions);

                    ClientContext.ExecuteQueryRetry();
                    WriteObject(subscriptions, true);
                }
                else
                {
                    WriteObject(list.GetWorkflowSubscription(Name));
                }
            }
            else
            {
                if (string.IsNullOrEmpty(Name))
                {
                    var servicesManager = new WorkflowServicesManager(ClientContext, SelectedWeb);
                    var subscriptionService = servicesManager.GetWorkflowSubscriptionService();
                    var subscriptions = subscriptionService.EnumerateSubscriptions();

                    ClientContext.Load(subscriptions);

                    ClientContext.ExecuteQueryRetry();
                    WriteObject(subscriptions, true);
                }
                else
                {
                    WriteObject(SelectedWeb.GetWorkflowSubscription(Name));
                }
            }
        }
    }

}
