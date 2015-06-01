using System;
using System.IO;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFile")]
    [CmdletHelp("Uploads a file to Web", Category = "Webs")]
    [CmdletExample(Code = @"
PS:> Add-SPOFile -Path c:\temp\company.master -Folder ""_catalogs/masterpage", Remarks = "This will upload the file company.master to the masterpage catalog")]
    [CmdletExample(Code = @"
PS:> Add-SPOFile -Path .\displaytemplate.html -Folder ""_catalogs/masterpage/display templates/test", Remarks = "This will upload the file displaytemplate.html to the test folder in the display templates folder. If the test folder not exists it will create it.")]
    public class AddFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The local file path.")]
        public string Path = string.Empty;

        [Parameter(Mandatory = true, HelpMessage = "The destination folder in the site")]
        public string Folder = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "If versioning is enabled, this will check out the file first if it exists, upload the file, then check it in again.")]
        public SwitchParameter Checkout;

        [Parameter(Mandatory = false, HelpMessage = "Will auto approve the uploaded file.")]
        public SwitchParameter Approve;

        [Parameter(Mandatory = false, HelpMessage = "The comment added to the approval.")]
        public string ApproveComment = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Will auto publish the file.")]
        public SwitchParameter Publish;

        [Parameter(Mandatory = false, HelpMessage = "The comment added to the publish action.")]
        public string PublishComment = string.Empty;

        [Parameter(Mandatory = false)]
        public SwitchParameter UseWebDav;

        protected override void ExecuteCmdlet()
        {
            if (!System.IO.Path.IsPathRooted(Path))
            {
                Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
            }

            if (!SelectedWeb.IsPropertyAvailable("ServerRelativeUrl"))
            {
                ClientContext.Load(SelectedWeb, w => w.ServerRelativeUrl);
                ClientContext.ExecuteQueryRetry();
            }

            var folder = SelectedWeb.EnsureFolder(SelectedWeb.RootFolder, Folder);
            //var folder = SelectedWeb.GetFolderByServerRelativeUrl(UrlUtility.Combine(SelectedWeb.ServerRelativeUrl, Folder));
            //ClientContext.Load(folder, f => f.ServerRelativeUrl);
            //ClientContext.ExecuteQueryRetry();

            var fileUrl = UrlUtility.Combine(folder.ServerRelativeUrl, System.IO.Path.GetFileName(Path));


            // Check if the file exists
            if (Checkout)
            {
                try
                {
                    var existingFile = SelectedWeb.GetFileByServerRelativeUrl(fileUrl);
                    if (existingFile.Exists)
                    {
                        SelectedWeb.CheckOutFile(fileUrl);
                    }
                }
                catch
                { // Swallow exception, file does not exist 
                }
            }

            folder.UploadFile(new FileInfo(Path).Name, Path, true);

            if (Checkout)
                SelectedWeb.CheckInFile(fileUrl, CheckinType.MajorCheckIn, "");

            if (Publish)
                SelectedWeb.PublishFile(fileUrl, PublishComment);

            if (Approve)
                SelectedWeb.ApproveFile(fileUrl, PublishComment);
        }
    }
}
