using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWikiPageContent")]
    public class GetWikiPageContent : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, ValueFromPipelineByPropertyName = true, Position=0)]
        [Alias("PageUrl")]
        public string ServerRelativePageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            WriteObject(SelectedWeb.GetWikiPageContent(ServerRelativePageUrl));
        }
    }
}
