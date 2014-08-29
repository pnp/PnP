using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Entities;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWeb")]
    public class GetWeb : SPOCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SPOWebPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity == null)
            {
                ClientContext.Load(ClientContext.Web);
                ClientContext.ExecuteQuery();
                WriteObject(new WebEntity(this.ClientContext.Web));
            }
            else
            {
                if (Identity.Id != null && Identity.Id != Guid.Empty)
                {
                    WriteObject(new WebEntity(ClientContext.Web.GetWebById(Identity.Id)));
                }
                else if (Identity.Web != null)
                {
                    WriteObject(new WebEntity(ClientContext.Web.GetWebById(Identity.Web.Id)));
                }
                else if (Identity.Url != null)
                {
                    WriteObject(new WebEntity(ClientContext.Web.GetWebByUrl(Identity.Url)));
                }
            }
        }

    }
}
