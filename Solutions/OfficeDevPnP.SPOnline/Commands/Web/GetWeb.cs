using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
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
                WriteObject(new SPOnlineWeb(SPOnline.Core.SPOWeb.GetWeb(ClientContext)));
            }
            else
            {
                if (Identity.Id != null && Identity.Id != Guid.Empty)
                {
                    WriteObject(new SPOnlineWeb(SPOnline.Core.SPOWeb.GetWebById(Identity.Id, ClientContext)));
                }
                else if (Identity.Web != null)
                {
                    WriteObject(new SPOnlineWeb(SPOnline.Core.SPOWeb.GetWebById(Identity.Web.Id, ClientContext)));
                }
                else if (Identity.Url != null)
                {
                    WriteObject(new SPOnlineWeb(SPOnline.Core.SPOWeb.GetWebByUrl(Identity.Url, ClientContext)));
                }
            }
        }
    }
}
