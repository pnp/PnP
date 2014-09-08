using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Add, "SPOContentType")]
    [CmdletHelp("Adds a new content type")]
    public class AddContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Name;

        [Parameter(Mandatory = false, HelpMessage="If specified, in the format of 0x0100233af432334r434343f32f3, will create a content type with the specific ID")]
        public string ContentTypeId;

        [Parameter(Mandatory = false)]
        public string Description;

        [Parameter(Mandatory = false)]
        public string Group;

        [Parameter(Mandatory = false)]
        public ContentType ParentContentType;

        protected override void ExecuteCmdlet()
        {
            var ct = this.SelectedWeb.CreateContentType(Name, Description, ContentTypeId, Group, ParentContentType);
            WriteObject(ct);
        }


    }
}
