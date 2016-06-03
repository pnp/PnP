namespace Core.UserProfiles.Sync
{
    using System;
    using System.Threading.Tasks;
    using Microsoft.Azure.ActiveDirectory.GraphClient;
    using Microsoft.IdentityModel.Clients.ActiveDirectory;
    using System.Configuration;

    public static class AuthenticationHelper
    {
        public static string TokenForUser;
        public const string ResourceUrl = "https://graph.windows.net";

        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication(Guid tenantId)
        {
            Uri servicePointUri = new Uri(ResourceUrl);
            Uri serviceRoot = new Uri(servicePointUri, tenantId.ToString());
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, GetTokenForApplicationAsync);
            return activeDirectoryClient;
        }

        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication(Uri sharePointAdminUrl)
        {
            Uri servicePointUri = new Uri(ResourceUrl);
            string adminRealm = TokenHelper.GetRealmFromTargetUrl(sharePointAdminUrl);
            Uri serviceRoot = new Uri(servicePointUri, adminRealm);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, GetTokenForApplicationAsync);
            return activeDirectoryClient;
        }

        /// <summary>
        /// Get Token for Application.
        /// </summary>
        /// <returns>Token for application.</returns>
        public static async Task<string> GetTokenForApplicationAsync()
        {
            var authenticationUrl = "https://login.windows.net/" + ConfigurationManager.AppSettings["TenantUpnDomain"];
            AuthenticationContext authenticationContext = new AuthenticationContext(authenticationUrl, false);

            // Config for OAuth client credentials 
            ClientCredential clientCred = new ClientCredential(
                ConfigurationManager.AppSettings["ClientId"],
                ConfigurationManager.AppSettings["ClientSecret"]);

            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(ResourceUrl,
                clientCred);
            string token = authenticationResult.AccessToken;
            return token;
        }
    }
}
