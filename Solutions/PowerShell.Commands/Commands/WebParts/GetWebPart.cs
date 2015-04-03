using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebPart")]
    [CmdletHelp("Returns a webpart definition object", Category = "Web Parts")]
    public class GetWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = false)]
        public WebPartPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var definitions = SelectedWeb.GetWebParts(PageUrl);

            if (Identity != null)
            {
                if (Identity.Id != Guid.Empty)
                {
                    var wpfound = from wp in definitions where wp.Id == Identity.Id select wp;
                    if(wpfound.Any())
                    {
                        WriteObject(wpfound.FirstOrDefault());

                    }
                }
                else if (!string.IsNullOrEmpty(Identity.Title))
                {
                    var wpfound = from wp in definitions where wp.WebPart.Title == Identity.Title select wp;
                    if (wpfound.Any())
                    {
                        WriteObject(wpfound.FirstOrDefault());
                    }
                }
            }
            else
            {
                WriteObject(definitions, true);
            }
        }
    }
}
