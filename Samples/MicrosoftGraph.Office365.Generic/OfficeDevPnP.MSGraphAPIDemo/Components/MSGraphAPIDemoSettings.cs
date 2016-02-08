using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class MSGraphAPIDemoSettings
    {
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string AADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static string PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        public static string Authority = AADInstance + TenantId;
        public static string MicrosoftGraphResourceId = "https://graph.microsoft.com";

        public static String Title = "Microsoft Graph API Demo";
    }
}