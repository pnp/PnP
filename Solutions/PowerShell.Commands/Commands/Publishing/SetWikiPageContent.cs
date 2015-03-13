using System.IO;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWikiPageContent")]
    [CmdletHelp("Sets the contents of a wikipage", Category = "Publishing")]
    public class SetWikiPageContent : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "STRING")]
        public string Content = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "FILE", HelpMessage="Site Relative Page Url")]
        [Parameter(Mandatory = true, ParameterSetName = "STRING", HelpMessage="Site Relative Page Url")]
        [Alias("PageUrl")]
        public string ServerRelativePageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "FILE")
            {
                if (File.Exists(Path))
                {
                    var fileStream = new StreamReader(Path);
                    var contentString = fileStream.ReadToEnd();
                    fileStream.Close();
                    SelectedWeb.AddHtmlToWikiPage(ServerRelativePageUrl, contentString);
                }
            }
            else
            {
                SelectedWeb.AddHtmlToWikiPage(ServerRelativePageUrl, Content);
            }
        }
    }
}
