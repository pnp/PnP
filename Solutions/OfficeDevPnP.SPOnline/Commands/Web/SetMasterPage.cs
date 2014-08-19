using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
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
            SPOnline.Core.SPOWeb.SetMasterPage(MasterPageUrl, CustomMasterPageUrl, this.SelectedWeb, ClientContext);
        }
    }
}
