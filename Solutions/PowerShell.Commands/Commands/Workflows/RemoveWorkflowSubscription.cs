using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Collections.Generic;
using System;

namespace OfficeDevPnP.PowerShell.Commands.Workflows
{
    [Cmdlet(VerbsCommon.Remove, "SPOWorkflowSubscription")]
    public class RemoveWorkflowSubscription : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The subscription to remove", Position = 0)]
        public WorkflowSubscriptionPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Subscription != null)
            {
                Identity.Subscription.Delete();
            }
            else if (Identity.Id != Guid.Empty)
            {
                var subscription = this.SelectedWeb.GetWorkflowSubscription(Identity.Id);
                if (subscription != null)
                    subscription.Delete();
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                var subscription = this.SelectedWeb.GetWorkflowSubscription(Identity.Name);
                if (subscription != null)
                    subscription.Delete();
            }
        }
    }

}
