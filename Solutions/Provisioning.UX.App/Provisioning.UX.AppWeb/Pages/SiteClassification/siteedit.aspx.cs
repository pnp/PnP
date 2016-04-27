using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Utilities;
using Provisioning.Common;
using Provisioning.Common.Authentication;
using Provisioning.Common.Configuration;
using Provisioning.UX.AppWeb.Controllers;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using Provisioning.UX.AppWeb.Models;
using OfficeDevPnP.Core.WebAPI;

namespace Provisioning.UX.AppWeb.Pages.SiteClassification
{
    public partial class SiteEdit : System.Web.UI.Page
    {
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
            // The following code gets the client context and Title property by using TokenHelper.
            // To access other properties, the app may need to request permissions on the host web.
            var spContext = SharePointContextProvider.Current.GetSharePointContext(Context);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                clientContext.Load(clientContext.Web, web => web.Title);
                clientContext.ExecuteQuery();
                //Response.Write(clientContext.Web.Title);
            }

            if (this.Request.Cookies[WebAPIHelper.SERVICES_TOKEN] == null)
            {
                //Register provisioning service
                Page.RegisterWebAPIService("api/provisioning");
            }


        }


    }
}