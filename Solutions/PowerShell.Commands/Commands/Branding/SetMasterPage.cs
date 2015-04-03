using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOMasterPage")]
    [CmdletHelp("Sets the default master page of the current web.", Category = "Branding")]
    [CmdletExample(
        Code = @"
    PS:> Set-SPOMasterPage -MasterPageUrl /sites/projects/_catalogs/masterpage/oslo.master
")]
    public class SetMasterPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public string MasterPageUrl = null;

        [Parameter(Mandatory = false)]
        public string CustomMasterPageUrl = null;

        protected override void ExecuteCmdlet()
        {
            if(!string.IsNullOrEmpty(MasterPageUrl))
                SelectedWeb.SetMasterPageByUrl(MasterPageUrl);

            if (!string.IsNullOrEmpty(CustomMasterPageUrl))
                SelectedWeb.SetCustomMasterPageByUrl(CustomMasterPageUrl);
            
        }
    }
}
