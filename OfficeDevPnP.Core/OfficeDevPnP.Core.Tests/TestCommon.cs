using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using System.Net;

namespace OfficeDevPnP.Core.Tests {
    static class TestCommon {
        static TestCommon() {
            TenantUrl = ConfigurationManager.AppSettings["SPOTenantUrl"];
            DevSiteUrl = ConfigurationManager.AppSettings["SPODevSiteUrl"];

            if (string.IsNullOrEmpty(TenantUrl) || string.IsNullOrEmpty(DevSiteUrl))
            {
                throw new ConfigurationErrorsException("Tenant credentials in App.config are not set up.");
            }


            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOCredentialManagerLabel"]))
            {
                Credentials = CredentialManager.GetCredential(ConfigurationManager.AppSettings["SPOCredentialManagerLabel"]);
            }
            else
            {
                UserName = ConfigurationManager.AppSettings["SPOUserName"];
                var password = ConfigurationManager.AppSettings["SPOPassword"];

                Password = password.ToSecureString();

                Credentials = new SharePointOnlineCredentials(UserName, Password);
            }
        }

        public static ClientContext CreateClientContext() {
            return CreateContext(DevSiteUrl, Credentials);
        }

        public static ClientContext CreateTenantClientContext() {
            return CreateContext(TenantUrl, Credentials);
        }

        private static ClientContext CreateContext(string contextUrl, ICredentials credentials)
        {
            var context = new ClientContext(contextUrl);
            context.Credentials = credentials;
            return context;
        }

        static string TenantUrl { get; set; }
        static string DevSiteUrl { get; set; }
        static string UserName { get; set; }
        static SecureString Password { get; set; }
        static ICredentials Credentials { get; set; }
    }
}
