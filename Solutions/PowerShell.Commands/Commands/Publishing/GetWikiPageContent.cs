using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWikiPageContent")]
    [CmdletHelp("Gets the contents/source of a wiki page", Category = "Publishing")]
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
