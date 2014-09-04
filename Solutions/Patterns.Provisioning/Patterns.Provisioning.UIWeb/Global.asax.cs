using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using System.Web.Security;
using System.Web.SessionState;

namespace Patterns.Provisioning.UIWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e) {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_PreRequestHandlerExecute(object sender, EventArgs e) {
            if (HttpContext.Current.Handler is MvcHandler) {
                Uri redirectUrl;
                switch (SharePointContextProvider.CheckRedirectionStatus(Context, out redirectUrl)) {
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
        }
    }
}