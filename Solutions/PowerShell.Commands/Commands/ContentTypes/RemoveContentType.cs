using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

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
            if (Force || ShouldContinue(Properties.Resources.RemoveContentType, Properties.Resources.Confirm))
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
