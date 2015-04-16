using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Add, "SPOContentTypeToList")]
    [CmdletHelp("Adds a new content type to a list", Category = "Content Types")]
    [CmdletExample(
     Code = @"PS:> Add-SPOContentTypeToList -List ""Documents"" -ContentType ""Project Document"" -DefaultContentType",
     Remarks = @"This will add an existing content type to a list and sets it as the default content type", SortOrder = 1)]
    public class AddContentTypeToList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = true)]
        public ContentTypePipeBind ContentType;

        [Parameter(Mandatory = false)]
        public SwitchParameter DefaultContentType;

        protected override void ExecuteCmdlet()
        {
            ContentType ct = null;
            List list = SelectedWeb.GetList(List);

            if (ContentType.ContentType == null)
            {
                if (ContentType.Id != null)
                {
                    ct = SelectedWeb.GetContentTypeById(ContentType.Id, true);
                }
                else if (ContentType.Name != null)
                {
                    ct = SelectedWeb.GetContentTypeByName(ContentType.Name, true);
                }
            }
            else
            {
                ct = ContentType.ContentType;
            }
            if (ct != null)
            {
                SelectedWeb.AddContentTypeToList(list.Title, ct, DefaultContentType);
            }
        }

    }
}
