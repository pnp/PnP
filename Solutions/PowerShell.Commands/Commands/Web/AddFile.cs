using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFile")]
    [CmdletHelp("Uploads a file to Web")]
    [CmdletExample(Code = @"
PS:> Add-SPOFile -Path c:\temp\company.master -Url /sites/")]
    public class AddFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The local file path.")]
        public string Path = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "The full server relative url, including the filename, of the destination location.", ParameterSetName = "Relative")]
        public string Url = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "The destination folder in the site", ParameterSetName = "Folder")]
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
            if (ParameterSetName == "Relative")
            {
                if (!Url.ToLower().EndsWith(System.IO.Path.GetFileName(Path).ToLower()))
                {
                    Url = UrlUtility.Combine(Url, System.IO.Path.GetFileName(Path));
                }
            }
            else
            {
                Url = UrlUtility.Combine(Folder, System.IO.Path.GetFileName(Path));
            }

            // Check if the file exists
            if (Checkout)
            {
                try
                {
                    var existingFile = this.SelectedWeb.GetFileByServerRelativeUrl(Url);
                    if (existingFile.Exists)
                    {
                        this.SelectedWeb.CheckOutFile(Url);
                    }
                }
                catch
                { // Swallow exception, file does not exist 
                }
            }
            if (ParameterSetName == "Folder")
            {
                this.SelectedWeb.UploadDocumentToFolder(Path, Folder);
            }
            else
            {
                this.SelectedWeb.UploadFileToServerRelativeUrl(Path, Url, UseWebDav);
            }

            if (Checkout)
                this.SelectedWeb.CheckInFile(Url, CheckinType.MajorCheckIn, "");

            if (Publish)
                this.SelectedWeb.PublishFile(Url, PublishComment);

            if (Approve)
                this.SelectedWeb.ApproveFile(Url, PublishComment);
        }
    }
}
