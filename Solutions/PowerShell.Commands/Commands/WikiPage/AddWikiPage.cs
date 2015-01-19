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
                        SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl, Content);
                        break;
                    }
                case "WithLayout":
                    {
                        SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl);
                        SelectedWeb.AddLayoutToWikiPage(Layout, ServerRelativePageUrl);
                        break;
                    }
                default:
                    {
                        SelectedWeb.AddWikiPageByUrl(ServerRelativePageUrl);
                        break;
                    }
            }
        }
    }
}
