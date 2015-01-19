using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOCustomAction", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveCustomAction, Properties.Resources.Confirm))
                {
                    SelectedWeb.DeleteCustomAction(Identity.Id);
                }
            }
        }
    }
}
