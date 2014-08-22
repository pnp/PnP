using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWikiPage", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            PowerShell.Core.SPOWikiPage.RemoveWikiPage(PageUrl, this.SelectedWeb, ClientContext);
        }
    }
}
