using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System;

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
                var allinstances = this.SelectedWeb.GetWorkflowInstances();
                foreach(var instance in allinstances)
                {
                    if(instance.Id == Identity.Id)
                    {
                        instance.ResumeWorkflow();
                        break;
                    }
                }
            }
        }
    }


}
