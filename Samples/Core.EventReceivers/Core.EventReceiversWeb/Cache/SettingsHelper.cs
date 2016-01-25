using System;
using System.Configuration;

namespace Contoso.Core.EventReceiversWeb.Cache
{
    public class SettingsHelper
    {
        public static bool UseAzureRedisCache
        {
            get { return Convert.ToBoolean(ConfigurationManager.AppSettings["UseAzureRedisForCache"]); }
        }

        public static string AzureRedisCache
        {
            get { return ConfigurationManager.AppSettings["AzureRedisCache"]; }
        }
    }
}