using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;
using OfficeDevPnP.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOWikiPage")]
    public class AddWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("PageUrl")]
        public string ServerRelativePageUrl = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "WithContent")]
        public string Content = null;

        [Parameter(Mandatory = false, ParameterSetName = "WithLayout")]
        public WikiPageLayout Layout;

        protected override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case "WithContent":
                    {
                        this.SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl, Content);
                        break;
                    }
                case "WithLayout":
                    {
                        this.SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl);
                        this.SelectedWeb.AddLayoutToWikiPage(Layout, ServerRelativePageUrl);
                        break;
                    }
                default:
                    {
                        this.SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl);
                        break;
                    }
            }
            //if (string.IsNullOrEmpty(Content))
            //{
            //    PowerShell.Core.SPOWikiPage.AddWikiPage(PageUrl, this.SelectedWeb, ClientContext);
            //}
            //else
            //{
            //    PowerShell.Core.SPOWikiPage.AddWikiPage(PageUrl, this.SelectedWeb, ClientContext, Content);
            //}
        }
    }
}
