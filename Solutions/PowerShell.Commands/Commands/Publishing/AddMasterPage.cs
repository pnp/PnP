using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOMasterPage")]
    [CmdletHelp("Adds a Masterpage", Category = "Publishing")]
    [CmdletExample(
        Code = @"PS:> Add-SPOPublishingMasterpage -SourceFilePath ""page.master"" -Title ""MasterPage"" -Description ""MasterPage for Web"" -DestinationFolderHierarchy ""SubFolder""",
        Remarks = "Add's a MasterPage to the web",
        SortOrder = 1)]
    public class AddMasterPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "Path to the file which will be uploaded")]
        public string SourceFilePath = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "Title for the page layout")]
        public string Title = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "Description for the page layout")]
        public string Description = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Folder hierarchy where the MasterPage layouts will be deployed")]
        public string DestinationFolderHierarchy;

        [Parameter(Mandatory = false, HelpMessage = "UiVersion Masterpage. Default = 15")]
        public string UiVersion;

        [Parameter(Mandatory = false, HelpMessage = "Defautl CSS file for MasterPage, SiteRelative")]
        public string DefaultCssFile;

        protected override void ExecuteCmdlet()
        {
            if (!System.IO.Path.IsPathRooted(SourceFilePath))
            {
                SourceFilePath = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, SourceFilePath);
            }

            SelectedWeb.DeployMasterPage(SourceFilePath, Title, Description, UiVersion, DefaultCssFile, DestinationFolderHierarchy);
        }
    }
}
