namespace OfficeDevPnP.Core.Framework.Authentication
{
    public static class SharePointAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for SharePointAuthenticationOptions.AuthenticationScheme
        /// </summary>
        public static readonly string AuthenticationScheme = typeof(SharePointAuthenticationDefaults).Assembly.GetName().Name;
        public static readonly string DisplayName = "SharePointAuthentication";        
    }
}
