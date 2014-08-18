using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWikiPage", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SPOnline.Core.SPOWikiPage.RemoveWikiPage(PageUrl, this.SelectedWeb, ClientContext);
        }
    }
}
