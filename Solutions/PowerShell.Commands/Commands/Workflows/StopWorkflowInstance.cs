using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System;
using System.Linq;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsLifecycle.Stop, "SPOWorkflowInstance")]
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
                var allinstances = this.SelectedWeb.GetWorkflowInstances();
                foreach (var instance in allinstances.Where(instance => instance.Id == Identity.Id))
                {
                    instance.CancelWorkFlow();
                    break;
                }
            }
        }
    }


}
