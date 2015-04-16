using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Set, "SPODefaultContentTypeToList")]
    [CmdletHelp("Sets the default content type for a list", Category = "Content Types")]
    [CmdletExample(
     Code = @"PS:> Set-SPODefaultContentTypeToList -List ""Project Documents"" -ContentType ""Project""",
     Remarks = @"This will set the Project content type (which has already been added to a list) as the default content type", SortOrder = 1)]
    public class SetDefaultContentTypeToList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = true)]
        public ContentTypePipeBind ContentType;

        protected override void ExecuteCmdlet()
        {
            ContentType ct = null;
            List list = SelectedWeb.GetList(List);

            if (ContentType.ContentType == null)
            {
                if (ContentType.Id != null)
                {
                    ct = SelectedWeb.GetContentTypeById(ContentType.Id,true);
                }
                else if (ContentType.Name != null)
                {
                    ct = SelectedWeb.GetContentTypeByName(ContentType.Name,true);
                }
            }
            else
            {
                ct = ContentType.ContentType;
            }
            if (ct != null)
            {
                SelectedWeb.SetDefaultContentTypeToList(list, ct);
            }
        }

    }
}
