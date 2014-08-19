using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsLifecycle.Request, "SPOReIndexWeb")]
    public class RequestReIndexWeb : SPOCmdlet
    {

        [Parameter(Mandatory = false)]
        public SPOWebPipeBind Web = new SPOWebPipeBind();


        protected override void ExecuteCmdlet()
        {
            Web web = ClientContext.Web;

            if (Web.Id != Guid.Empty)
            {
                web = SPOWeb.GetWebById(Web.Id, ClientContext);
            }
            else if (!string.IsNullOrEmpty(Web.Url))
            {
                web = SPOWeb.GetWebByUrl(Web.Url, ClientContext);
            }
            else if (Web.Web != null)
            {
                web = Web.Web;

            }

            SPOnline.Core.SPOWeb.ReIndex(web, ClientContext);
        }
    }
}
