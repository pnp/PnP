using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Utils
{
    public class SettingsHelper
    {
        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["ida:ClientID"]; }
        }

        public static string ClientSecret
        {
            get { return ConfigurationManager.AppSettings["ida:Password"]; }
        }

        public static string AzureAdTenantId
        {
            get { return ConfigurationManager.AppSettings["ida:TenantId"]; }
        }

        public static string O365DiscoveryServiceEndpoint
        {
            get { return "https://api.office.com/discovery/v1.0/me/"; }
        }

        public static string O365DiscoveryResourceId
        {
            get { return "https://api.office.com/discovery/"; }
        }

        public static string AzureAdGraphResourceId
        {
            get { return "https://graph.windows.net"; }
        }


        public static string ClaimTypeObjectIdentifier
        {
            get { return "http://schemas.microsoft.com/identity/claims/objectidentifier"; }
        }

        public static string AzureRedisCache
        {
            get { return ConfigurationManager.AppSettings["AzureRedisCache"]; }
        }

        public static string AzureAdGraphApiEndPoint
        {
            get { return ConfigurationManager.AppSettings["ida:AzureAdGraphApiEndPoint"]; }
        }

        public static string AzureADAuthority
        {
            get { return string.Format(ConfigurationManager.AppSettings["ida:AADInstance"], AzureAdTenantId) ; }
        }

        public static string Tenant
        {
            get { return ConfigurationManager.AppSettings["ida:Tenant"]; }
        }
        public static string GraphResourceId
        {
            get { return ConfigurationManager.AppSettings["ida:GraphResourceId"]; }
        }

        public static int CacheUserProfileMinutes
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["CacheUserProfileMinutes"].ToString()); }
        }

        public static int CacheModuleNames
        {
            get { return Convert.ToInt32(ConfigurationManager.AppSettings["CacheModuleNames"].ToString()); }
        }

        public static string GraphAPIVersion
        {
            get { return ConfigurationManager.AppSettings["ida:GraphAPIVersion"]; }
        }

    }
}