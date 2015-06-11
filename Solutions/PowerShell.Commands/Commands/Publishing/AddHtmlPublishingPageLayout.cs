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
        [Parameter(Mandatory = true, HelpMessage = "Path to the file which will be uploaded")]
        public string SourceFilePath = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "Title for the page layout")]
        public string Title = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "Description for the page layout")]
        public string Description = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "Associated content type ID")]
        public string AssociatedContentTypeID;

        [Parameter(Mandatory = false, HelpMessage = "Folder hierarchy where the html page layouts will be deployed")]
        public string DestinationFolderHierarchy;

        protected override void ExecuteCmdlet()
        {
            if (!System.IO.Path.IsPathRooted(SourceFilePath))
            {
                SourceFilePath = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, SourceFilePath);
            }
            SelectedWeb.DeployHtmlPageLayout(SourceFilePath, Title, Description, AssociatedContentTypeID, DestinationFolderHierarchy);
        }
    }
}
