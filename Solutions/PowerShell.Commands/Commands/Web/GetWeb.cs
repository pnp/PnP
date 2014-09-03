using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWeb")]
    public class GetWeb : SPOCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position=0)]
        public WebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity == null)
            {
                ClientContext.Load(ClientContext.Web);
                ClientContext.ExecuteQuery();
                WriteObject(this.ClientContext.Web);
            }
            else
            {
                if (Identity.Id != null && Identity.Id != Guid.Empty)
                {
                    WriteObject(ClientContext.Web.GetWebById(Identity.Id));
                }
                else if (Identity.Web != null)
                {
                    WriteObject(ClientContext.Web.GetWebById(Identity.Web.Id));
                }
                else if (Identity.Url != null)
                {
                    WriteObject(ClientContext.Web.GetWebByUrl(Identity.Url));
                }
            }
        }

    }
}
