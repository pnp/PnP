using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOContentType")]
    [CmdletHelp("Retrieves a content type")]
    [CmdletExample(
     Code = @"PS:> Get-SPOContentType -Identity ""Project Document""",
     Remarks = @"This will get a listing of content types within the current context", SortOrder = 1)]
    public class GetContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position=0, ValueFromPipeline=true, HelpMessage="Name or ID of the content type to retrieve")]
        public ContentTypePipeBind Identity;

        protected override void ExecuteCmdlet()
        {

            if (Identity != null)
            {
                ContentType ct;
                if (!string.IsNullOrEmpty(Identity.Id))
                {
                    ct = SelectedWeb.GetContentTypeById(Identity.Id);
                }
                else
                {
                    ct = SelectedWeb.GetContentTypeByName(Identity.Name);
                }
                if (ct != null)
                {

                    WriteObject(ct);
                }
            }
            else
            {
                var cts = ClientContext.LoadQuery(SelectedWeb.ContentTypes);
                ClientContext.ExecuteQueryRetry();
    
                WriteObject(cts, true);
            }
        }
    }
}
