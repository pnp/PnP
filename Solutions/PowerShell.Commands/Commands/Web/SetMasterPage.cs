using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

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
                this.SelectedWeb.SetMasterPageByUrl(MasterPageUrl);

            if (!string.IsNullOrEmpty(CustomMasterPageUrl))
                this.SelectedWeb.SetCustomMasterPageByUrl(CustomMasterPageUrl);
            
        }
    }
}
