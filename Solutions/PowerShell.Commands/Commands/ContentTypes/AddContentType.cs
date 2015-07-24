using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Add, "SPOContentType")]
    [CmdletHelp("Adds a new content type", Category = "Content Types")]
    [CmdletExample(
      Code = @"PS:> Add-SPOContentType -Name ""Project Document"" -Description ""Use for Contoso projects"" -Group ""Contoso Content Types"" -ParentContentType $ct",
      Remarks = @"This will add a new content type based on the parent content type stored in the $ct variable.", SortOrder = 1)]
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
            var ct = SelectedWeb.CreateContentType(Name, Description, ContentTypeId, Group, ParentContentType);
            ClientContext.Load(ct);
            ClientContext.ExecuteQueryRetry();
            WriteObject(ct);
        }


    }
}
