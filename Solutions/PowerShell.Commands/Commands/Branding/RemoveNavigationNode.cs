using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Enums;
using System.Management.Automation;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPONavigationNode", SupportsShouldProcess = true)]
    [CmdletHelp("Removes a menu item from either the quicklaunch or top navigation", Category = "Branding")]
    public class RemoveNavigationNode : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public NavigationType Location;

        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = false)]
        public string Header;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Force || ShouldContinue(string.Format(Resources.RemoveNavigationNode0, Title), Resources.Confirm))
            {
                SelectedWeb.DeleteNavigationNode(Title, Header, Location);
            }
        }

    }


}
