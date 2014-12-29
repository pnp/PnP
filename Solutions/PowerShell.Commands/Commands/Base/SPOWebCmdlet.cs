using System;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOWebCmdlet : SPOCmdlet
    {
        [Parameter(Mandatory = false)]
        public WebPipeBind Web = new WebPipeBind();

        internal Microsoft.SharePoint.Client.Web SelectedWeb
        {
            get
            {
                return GetWeb();
            }
        }

        private Microsoft.SharePoint.Client.Web GetWeb()
        {
            Microsoft.SharePoint.Client.Web web = ClientContext.Web;

            if (Web.Id != Guid.Empty)
            {
                web = web.GetWebById(Web.Id);
                SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else if (!string.IsNullOrEmpty(Web.Url))
            {
                web = web.GetWebByUrl(Web.Url);
                SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else if (Web.Web != null)
            {
                web = Web.Web;
                if (!web.IsPropertyAvailable("Url"))
                {
                    ClientContext.Load(web, w => w.Url);
                    ClientContext.ExecuteQuery();
                }
                SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else
            {
                if (SPOnlineConnection.CurrentConnection.Context.Url != SPOnlineConnection.CurrentConnection.Url)
                {
                    SPOnlineConnection.CurrentConnection.Context = this.ClientContext.Clone(SPOnlineConnection.CurrentConnection.Url);
                }
                web = ClientContext.Web;
            }


            return web;
        }
    }
}
