using System.Configuration;

namespace BusinessApps.HelpDesk.Helpers
{
    public class SettingsHelper
    {
        public static string ClientId => ConfigurationManager.AppSettings["AzureId"];
        public static string CertThumbprint => ConfigurationManager.AppSettings["CertThumbprint"];

        public static string AzureADAuthority => @"https://login.windows.net/" + ConfigurationManager.AppSettings["Tenant"];
        public static string AzureADAuthAuthority => @"https://login.windows.net/" + ConfigurationManager.AppSettings["Tenant"] + "/oauth2/token";
        public static string LogoutAuthority => @"https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=";
        public static string GraphUrl => @"https://graph.microsoft.com/v1.0/" + ConfigurationManager.AppSettings["Tenant"] + "/";

        public static string GraphResource => @"https://graph.microsoft.com/";
        public static string SharePointResource => ConfigurationManager.AppSettings["SharePointSite"];

        public static string HelpDeskEmailAddress => ConfigurationManager.AppSettings["HelpDeskEmailAddress"];
    }
}
