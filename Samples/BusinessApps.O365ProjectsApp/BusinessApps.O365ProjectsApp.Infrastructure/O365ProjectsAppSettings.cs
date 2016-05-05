using BusinessApps.O365ProjectsApp.Infrastructure.Configuration;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography.X509Certificates;
using System.Web;

namespace BusinessApps.O365ProjectsApp.Infrastructure
{
    public static class O365ProjectsAppSettings
    {
        public static string ClientId = ConfigurationManager.AppSettings["ida:ClientId"];
        public static string ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static string AADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static string MicrosoftGraphResourceId = "https://graph.microsoft.com";
        public static string PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        public static string TenantId = ConfigurationManager.AppSettings["ida:TenantId"];

        private static O365ProjectsConfiguration _configuration =
            (O365ProjectsConfiguration)ConfigurationManager.GetSection("O365ProjectsConfiguration");

        private static readonly Lazy<X509Certificate2> _appOnlyCertificateLazy =
            new Lazy<X509Certificate2>(() =>
            {

                X509Certificate2 appOnlyCertificate = null;

                StoreName storeName;
                StoreLocation storeLocation;

                Enum.TryParse(_configuration.CertificateSettings.storeName,
                    out storeName);
                Enum.TryParse(_configuration.CertificateSettings.storeLocation,
                    out storeLocation);

                X509Store certStore = new X509Store(storeName, storeLocation);
                certStore.Open(OpenFlags.ReadOnly);

                X509Certificate2Collection certCollection = certStore.Certificates.Find(
                    X509FindType.FindByThumbprint,
                    _configuration.GeneralSettings.appOnlyCertificateThumbprint,
                    false);

                // Get the first cert with the thumbprint
                if (certCollection.Count > 0)
                {
                    appOnlyCertificate = certCollection[0];
                }
                certStore.Close();

                return (appOnlyCertificate);
            });

        /// <summary>
        /// Provides the X.509 certificate for Azure AD AppOnly Authentication
        /// </summary>
        public static X509Certificate2 AppOnlyCertificate
        {
            get
            {
                return (_appOnlyCertificateLazy.Value);
            }
        }

        /// <summary>
        /// Provides the Title of the target library in the Site Collection
        /// </summary>
        public static String LibraryTitle
        {
            get
            {
                return (_configuration.GeneralSettings.libraryTitle);
            }
        }

        /// <summary>
        /// Provides the URL of the default Site Collection
        /// </summary>
        public static String DefaultSiteUrl
        {
            get
            {
                return (_configuration.GeneralSettings.defaultSiteUrl);
            }
        }
    }
}