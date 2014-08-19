using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWikiPage")]
    public class AddWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = false)]
        public string Content = null;

        protected override void ExecuteCmdlet()
        {
            if (string.IsNullOrEmpty(Content))
            {
                SPOnline.Core.SPOWikiPage.AddWikiPage(PageUrl, this.SelectedWeb, ClientContext);
            }
            else
            {
                SPOnline.Core.SPOWikiPage.AddWikiPage(PageUrl, this.SelectedWeb, ClientContext, Content);
            }
        }
    }
}
