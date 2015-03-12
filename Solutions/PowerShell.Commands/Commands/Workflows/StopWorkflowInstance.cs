using System;
using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsLifecycle.Stop, "SPOWorkflowInstance")]
    [CmdletHelp("Stops a workflow instance", Category = "Workflows")]

    public class StopWorkflowInstance : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The instance to stop", Position = 0)]
        public WorkflowInstancePipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Instance != null)
            {
                Identity.Instance.CancelWorkFlow();
            }
            else if (Identity.Id != Guid.Empty)
            {
                var allinstances = SelectedWeb.GetWorkflowInstances();
                foreach (var instance in allinstances.Where(instance => instance.Id == Identity.Id))
                {
                    instance.CancelWorkFlow();
                    break;
                }
            }
        }
    }


}
