using System;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;

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
            }
            else if (!string.IsNullOrEmpty(Web.Url))
            {
                web = web.GetWebByUrl(Web.Url);
            }
            else if (Web.Web != null)
            {
                web = Web.Web;
            }
            return web;
        }
    }
}
