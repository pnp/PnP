using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Get, "SPOWorkflowDefinition")]
    [CmdletHelp("Returns a workflow definition", Category = "Workflows")]

    public class GetWorkflowDefinition : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The name of the workflow", Position = 0)]
        public string Name;

        [Parameter(Mandatory = false)]
        public SwitchParameter PublishedOnly = true;

        protected override void ExecuteCmdlet()
        {
            if (string.IsNullOrEmpty(Name))
            {
                var servicesManager = new WorkflowServicesManager(ClientContext, SelectedWeb);
                var deploymentService = servicesManager.GetWorkflowDeploymentService();
                var definitions = deploymentService.EnumerateDefinitions(PublishedOnly);

                ClientContext.Load(definitions);

                ClientContext.ExecuteQueryRetry();
                WriteObject(definitions, true);
            }
            else
            {
                WriteObject(SelectedWeb.GetWorkflowDefinition(Name, PublishedOnly));
            }
        }
    }

}
