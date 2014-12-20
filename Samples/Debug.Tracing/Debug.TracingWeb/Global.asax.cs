using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.SessionState;

namespace Debug.TracingWeb
{
    public class Global : System.Web.HttpApplication
    {
        protected void Application_Start(object sender, EventArgs e)
        {
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            Exception eX = Server.GetLastError().GetBaseException(); //get exception
            ErrorLogger.LogException(eX); //log it to trace.axd and Sharepoint
        }
    }
}