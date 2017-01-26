namespace OfficeDevPnP.Core.Framework.Authentication
{
    public static class SharePointAuthenticationDefaults
    {
        /// <summary>
        /// The default value used for SharePointAuthenticationOptions.AuthenticationScheme
        /// </summary>
        public static string AuthenticationScheme = typeof(SharePointAuthenticationDefaults).Assembly.GetName().Name;

        public const bool AutomaticChallenge = false;

        public const bool AutomaticAuthenticate = false;
    }
}
