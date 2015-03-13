using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.PageLayout
{
    [Cmdlet(VerbsCommon.Add, "SPOHtmlPublishingPageLayout")]
    [CmdletHelp("Adds a HTML based publishing page layout",
       Category = "Publishing")]
    public class AddHtmlPublishingPageLayout : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Full path to the file which will be uploaded")]
        public string SourceFilePath = string.Empty;

        [Parameter(Mandatory = true)]
        public string Title = string.Empty;

        [Parameter(Mandatory = true)]
        public string Description = string.Empty;

        [Parameter(Mandatory = true)]
        public string AssociatedContentTypeID;

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.DeployHtmlPageLayout(SourceFilePath, Title, Description, AssociatedContentTypeID);
        }
    }
}
