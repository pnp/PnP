using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOAppSideLoading")]
    [CmdletHelp("Enables the App Side Loading Feature on a site", Category = "Sites")]
    public class SetAppSideLoading : SPOCmdlet
    {
        [Parameter(ParameterSetName = "On", Mandatory = true)]
        public SwitchParameter On;

        [Parameter(ParameterSetName = "Off", Mandatory = true)]
        public SwitchParameter Off;
        protected override void ExecuteCmdlet()
        {
            if (On)
            {
                ClientContext.Site.ActivateFeature(Constants.APPSIDELOADINGFEATUREID);
            }
            else
            {
                ClientContext.Site.DeactivateFeature(Constants.APPSIDELOADINGFEATUREID);
            }
        }

    }
}
