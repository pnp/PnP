using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;
using System.Web.Http;

namespace Core.ConnectedAngularAppsV2Web
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
            // WebApi support
            //GlobalConfiguration.Configure(WebApiConfig.Register);
        }
    }
}