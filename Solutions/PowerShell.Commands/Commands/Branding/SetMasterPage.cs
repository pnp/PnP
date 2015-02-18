using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOMasterPage")]
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
