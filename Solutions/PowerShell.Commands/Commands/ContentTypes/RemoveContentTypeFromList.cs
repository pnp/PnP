using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Remove, "SPOContentTypeFromList")]
    [CmdletHelp("Removes a content type from a list")]
    public class RemoveContentTypeFromList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = true)]
        public ContentTypePipeBind ContentType;

        protected override void ExecuteCmdlet()
        {
            ContentType ct = null;
            List list = this.SelectedWeb.GetList(List);

            if (ContentType.ContentType == null)
            {
                if (ContentType.Id != null)
                {
                    ct = this.SelectedWeb.GetContentTypeById(ContentType.Id);
                }
                else if (ContentType.Name != null)
                {
                    ct = this.SelectedWeb.GetContentTypeByName(ContentType.Name);
                }
            }
            else
            {
                ct = ContentType.ContentType;
            }
            if (ct != null)
            {
                this.SelectedWeb.RemoveContentTypeFromList(list, ct);
            }
        }

    }
}
