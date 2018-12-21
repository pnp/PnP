using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace SPOGraphConsumer.Components
{
    public static class GraphSettings
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string spoTenant = ConfigurationManager.AppSettings["ida:SpoTenant"];
        private static string authority = aadInstance + tenantId;

        public static String GraphResourceId = "https://graph.microsoft.com/";
        public static String MicrosoftGraphV1BaseUri = "https://graph.microsoft.com/v1.0/";
        public static String MicrosoftGraphBetaBaseUri = "https://graph.microsoft.com/beta/";

        public static String ClientId
        {
            get
            {
                return (clientId);
            }
        }

        public static String ClientSecret
        {
            get
            {
                return (clientSecret);
            }
        }

        public static String AadInstance
        {
            get
            {
                return (aadInstance);
            }
        }

        public static String TenantId
        {
            get
            {
                return (tenantId);
            }
        }

        public static String SpoTenant
        {
            get
            {
                return (spoTenant);
            }
        }

        public static String PostLogoutRedirectUri
        {
            get
            {
                return (postLogoutRedirectUri);
            }
        }

        public static String Authority
        {
            get
            {
                return (authority);
            }
        }
    }
}