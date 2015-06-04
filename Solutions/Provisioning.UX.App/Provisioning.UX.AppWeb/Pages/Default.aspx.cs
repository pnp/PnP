using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using OfficeDevPnP.Core.WebAPI;
using Microsoft.SharePoint.Client;
using Provisioning.Common.Utilities;

namespace Provisioning.UX.AppWeb
{
    public partial class Default : System.Web.UI.Page
    {
        private ClientContext ctx;

        protected void Page_PreInit(object sender, EventArgs e)
        {
            Uri redirectUrl;
            switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl))
            {
                case RedirectionStatus.Ok:
                    return;
                case RedirectionStatus.ShouldRedirect:
                    Response.Redirect(redirectUrl.AbsoluteUri, endResponse: true);
                    break;
                case RedirectionStatus.CanNotRedirect:
                    Response.Write("An error occurred while processing your request.");
                    Response.End();
                    break;
            }
        }

        protected void Page_Load(object sender, EventArgs e)
        {
            if(this.Request.Cookies[WebAPIHelper.SERVICES_TOKEN] == null)
            {
                //Register provisioning service
                Page.RegisterWebAPIService("api/provisioning");
            }

        }
    }
}