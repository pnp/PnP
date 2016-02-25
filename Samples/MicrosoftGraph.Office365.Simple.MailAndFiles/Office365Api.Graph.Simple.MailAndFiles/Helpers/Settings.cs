using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;

namespace Office365Api.Graph.Simple.MailAndFiles.Helpers
{
    /// <summary>
    /// Settings helper class
    /// </summary>
    public static class Settings
    {
        public static string ClientId => ConfigurationManager.AppSettings["ClientID"];
        public static string ClientSecret => ConfigurationManager.AppSettings["ClientSecret"];

        public static string AzureADAuthority = @"https://login.microsoftonline.com/common";
        public static string LogoutAuthority = @"https://login.microsoftonline.com/common/oauth2/logout?post_logout_redirect_uri=";
        public static string O365UnifiedAPIResource = @"https://graph.microsoft.com/";

        public static string GetMeUrl = @"https://graph.microsoft.com/v1.0/me";
        public static string GetMyFilesUrl = @"https://graph.microsoft.com/v1.0/me/drive/root/children";
        public static string GetMyEmails = @"https://graph.microsoft.com/v1.0/me/messages";
    }
}