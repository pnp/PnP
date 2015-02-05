using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWikiPage", ConfirmImpact = ConfirmImpact.High)]
    public class RemoveWikiPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position=0,ValueFromPipeline=true)]
        [Alias("PageUrl")]
        public string ServerRelativePageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            var file = SelectedWeb.GetFileByServerRelativeUrl(ServerRelativePageUrl);

            file.DeleteObject();

            ClientContext.ExecuteQueryRetry();
        }
    }
}
