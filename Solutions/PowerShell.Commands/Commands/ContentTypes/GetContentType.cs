using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOContentType")]
    public class GetContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position=0, ValueFromPipeline=true)]
        public ContentTypePipeBind Identity;

        protected override void ExecuteCmdlet()
        {

            if (Identity != null)
            {
                ContentType ct = null;
                if (!string.IsNullOrEmpty(Identity.Id))
                {
                    ct = this.SelectedWeb.GetContentTypeById(Identity.Id);
                }
                else
                {
                    ct = this.SelectedWeb.GetContentTypeByName(Identity.Name);
                }
                if (ct != null)
                {

                    WriteObject(ct);
                }
            }
            else
            {
                var cts = ClientContext.LoadQuery(this.SelectedWeb.ContentTypes);
                ClientContext.ExecuteQuery();
    
                WriteObject(cts, true);
            }
        }
    }
}
