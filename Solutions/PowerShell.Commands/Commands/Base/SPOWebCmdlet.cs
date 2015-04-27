using System;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    public class SPOWebCmdlet : SPOCmdlet
    {
        private Web _selectedWeb;


        [Parameter(Mandatory = false, HelpMessage = "The web to apply the command to. Leave empty to use the current web.")]
        public WebPipeBind Web = new WebPipeBind();

        internal Web SelectedWeb
        {
            get
            {
                if (_selectedWeb == null)
                {
                    _selectedWeb = GetWeb();
                }
                return _selectedWeb;
            }
        }

        private Web GetWeb()
        {
            Web web = ClientContext.Web;

            if (Web.Id != Guid.Empty)
            {
                web = web.GetWebById(Web.Id);
                SPOnlineConnection.CurrentConnection.Context = ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else if (!string.IsNullOrEmpty(Web.Url))
            {
                web = web.GetWebByUrl(Web.Url);
                SPOnlineConnection.CurrentConnection.Context = ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else if (Web.Web != null)
            {
                web = Web.Web;
                if (!web.IsPropertyAvailable("Url"))
                {
                    ClientContext.Load(web, w => w.Url);
                    ClientContext.ExecuteQueryRetry();
                }
                SPOnlineConnection.CurrentConnection.Context = ClientContext.Clone(web.Url);
                web = SPOnlineConnection.CurrentConnection.Context.Web;
            }
            else
            {
                if (SPOnlineConnection.CurrentConnection.Context.Url != SPOnlineConnection.CurrentConnection.Url)
                {
                    SPOnlineConnection.CurrentConnection.RestoreCachedContext();
                }
                web = ClientContext.Web;
            }


            return web;
        }

        protected override void EndProcessing()
        {
            base.EndProcessing();
            if (SPOnlineConnection.CurrentConnection.Context.Url != SPOnlineConnection.CurrentConnection.Url)
            {
                SPOnlineConnection.CurrentConnection.RestoreCachedContext();
            }
        }

        protected override void BeginProcessing()
        {
            base.BeginProcessing();
            SPOnlineConnection.CurrentConnection.CacheContext();
        }

    }
}