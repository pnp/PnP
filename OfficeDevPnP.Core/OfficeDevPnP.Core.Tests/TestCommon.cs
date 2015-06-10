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
    static class TestCommon
    {
        #region Constructor
        static TestCommon() 
        {
            // Read configuration data
            TenantUrl = ConfigurationManager.AppSettings["SPOTenantUrl"];
            DevSiteUrl = ConfigurationManager.AppSettings["SPODevSiteUrl"];

            if (string.IsNullOrEmpty(TenantUrl) || string.IsNullOrEmpty(DevSiteUrl))
            {
                throw new ConfigurationErrorsException("Tenant credentials in App.config are not set up.");
            }

            if (!string.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOCredentialManagerLabel"]))
            {
                Credentials = Core.Utilities.CredentialManager.GetSharePointOnlineCredential(ConfigurationManager.AppSettings["SPOCredentialManagerLabel"]);
            }
            else
            {
                if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOUserName"]) && 
                    !String.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOPassword"]))
                {
                    UserName = ConfigurationManager.AppSettings["SPOUserName"];
                    var password = ConfigurationManager.AppSettings["SPOPassword"];

                    Password = GetSecureString(password);
                    Credentials = new SharePointOnlineCredentials(UserName, Password);
                }
                else if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremUserName"]) &&
                         !String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremDomain"]) &&
                         !String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremPassword"]))
                {
                    Password = GetSecureString(ConfigurationManager.AppSettings["OnPremPassword"]);
                    Credentials = new NetworkCredential(ConfigurationManager.AppSettings["OnPremUserName"], Password, ConfigurationManager.AppSettings["OnPremDomain"]);
                }
                else if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["Realm"]) &&
                         !String.IsNullOrEmpty(ConfigurationManager.AppSettings["AppId"]) &&
                         !String.IsNullOrEmpty(ConfigurationManager.AppSettings["AppSecret"]))
                {
                    Realm = ConfigurationManager.AppSettings["Realm"];
                    AppId = ConfigurationManager.AppSettings["AppId"];
                    AppSecret = ConfigurationManager.AppSettings["AppSecret"];
                }
                else
                {
                    throw new ConfigurationErrorsException("Tenant credentials in App.config are not set up.");
                }
            }
        }
        #endregion

        #region Properties
        public static string TenantUrl { get; set; }
        public static string DevSiteUrl { get; set; }
        static string UserName { get; set; }
        static SecureString Password { get; set; }
        static ICredentials Credentials { get; set; }
        static string Realm { get; set; }
        static string AppId { get; set; }
        static string AppSecret { get; set; }

        public static String AzureStorageKey
        {
            get
            {
                return ConfigurationManager.AppSettings["AzureStorageKey"];
            }
        }
        #endregion

        #region Methods
        public static ClientContext CreateClientContext() {
            return CreateContext(DevSiteUrl, Credentials);
        }

        public static ClientContext CreateTenantClientContext() {
            return CreateContext(TenantUrl, Credentials);
        }

        public static bool AppOnlyTesting()
        {
            if (!String.IsNullOrEmpty(ConfigurationManager.AppSettings["Realm"]) &&
                !String.IsNullOrEmpty(ConfigurationManager.AppSettings["AppId"]) &&
                !String.IsNullOrEmpty(ConfigurationManager.AppSettings["AppSecret"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOCredentialManagerLabel"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOUserName"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["SPOPassword"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremUserName"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremDomain"]) &&
                String.IsNullOrEmpty(ConfigurationManager.AppSettings["OnPremPassword"]))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        private static ClientContext CreateContext(string contextUrl, ICredentials credentials)
        {
            ClientContext context;
            if (!String.IsNullOrEmpty(Realm) && !String.IsNullOrEmpty(AppId) && !String.IsNullOrEmpty(AppSecret))
            {
                AuthenticationManager am = new AuthenticationManager();
                context = am.GetAppOnlyAuthenticatedContext(contextUrl, Realm, AppId, AppSecret); 
            }
            else
            {
                context = new ClientContext(contextUrl);
                context.Credentials = credentials;
            }
            return context;
        }

        private static SecureString GetSecureString(string input)
        {
            if (string.IsNullOrEmpty(input))
                throw new ArgumentException("Input string is empty and cannot be made into a SecureString", "input");

            var secureString = new SecureString();
            foreach (char c in input.ToCharArray())
                secureString.AppendChar(c);

            return secureString;
        }
        #endregion
    }
}
