using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOContentType")]
    [CmdletHelp("Removes a content type")]
    [CmdletExample(
     Code = @"PS:> Remove-SPOContentType -Identity ""Project Document""")]
    public class RemoveContentType : SPOWebCmdlet
    {

        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true, HelpMessage="The name or ID of the content type to remove")]
        public ContentTypePipeBind Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Force || ShouldContinue(Resources.RemoveContentType, Resources.Confirm))
            {
                ContentType ct = null;
                if (Identity.ContentType != null)
                {
                    ct = Identity.ContentType;
                }
                else
                {
                    if (!string.IsNullOrEmpty(Identity.Id))
                    {
                        ct = SelectedWeb.GetContentTypeById(Identity.Id);
                    }
                    else if (!string.IsNullOrEmpty(Identity.Name))
                    {
                        ct = SelectedWeb.GetContentTypeByName(Identity.Id);
                    }
                }
                if(ct != null)
                {
                    ct.DeleteObject();
                    ClientContext.ExecuteQueryRetry();
                }

            }
        }
    }
}
