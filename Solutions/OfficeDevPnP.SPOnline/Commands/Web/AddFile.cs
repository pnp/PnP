using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOFile")]
    [CmdletHelp("Uploads a file to Web")]
    [CmdletExample(Code = @"
PS:> Add-SPOFile -Path c:\temp\company.master -Url /sites/")]
    public class AddFile : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The local file path.")]
        public string Path = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "The full server relative url, including the filename, of the destination location.")]
        public string Url = string.Empty;

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
            SPOnline.Core.SPOWeb.AddFile(Path, Url, this.SelectedWeb, Checkout, UseWebDav, ClientContext, publish: Publish, publishComment: PublishComment, approve: Approve, approveComment: ApproveComment);
        }
    }
}
