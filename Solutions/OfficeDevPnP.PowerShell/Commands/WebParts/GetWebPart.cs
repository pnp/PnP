using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Linq;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebPart")]
    public class GetWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = false)]
        public SPOWebPartPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                if (Identity.Id != Guid.Empty)
                {
                    WriteObject(new SPOnlineWebPart(SPOWebParts.GetWebPartById(PageUrl, this.SelectedWeb, Identity.Id, ClientContext)));
                }
                else if (!string.IsNullOrEmpty(Identity.Title))
                {
                    WriteObject(new SPOnlineWebPart(SPOWebParts.GetWebPartByTitle(PageUrl, Identity.Title, this.SelectedWeb, ClientContext)));
                }
            }
            else
            {
                var definitions = SPOWebParts.GetWebParts(PageUrl, this.SelectedWeb, ClientContext);

                foreach (var webpart in definitions)
                {
                    WriteObject(new SPOnlineWebPart(webpart));
                }

            }
        }
    }
}
