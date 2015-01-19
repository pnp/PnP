using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System;
using System.Linq;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsLifecycle.Resume, "SPOWorkflowInstance")]
    public class ResumeWorkflowInstance : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The instance to resume", Position = 0)]
        public WorkflowInstancePipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Instance != null)
            {
                Identity.Instance.ResumeWorkflow();
            }
            else if (Identity.Id != Guid.Empty)
            {
                var allinstances = SelectedWeb.GetWorkflowInstances();
                foreach (var instance in allinstances.Where(instance => instance.Id == Identity.Id))
                {
                    instance.ResumeWorkflow();
                    break;
                }
            }
        }
    }


}
