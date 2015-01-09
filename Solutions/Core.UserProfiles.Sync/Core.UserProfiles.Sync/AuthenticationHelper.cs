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
        /// <summary>
        /// Async task to acquire token for Application.
        /// </summary>
        /// <returns>Async Token for application.</returns>
        public static async Task<string> AcquireTokenAsyncForApplication()
        {
            return GetTokenForApplication();
        }

         public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication() 
         { 
             Uri servicePointUri = new Uri(ResourceUrl); 
             Uri serviceRoot = new Uri(servicePointUri, ConfigurationManager.AppSettings["TenantId"]); 
             ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot, 
                 async () => await AcquireTokenAsyncForApplication()); 
             return activeDirectoryClient; 
         } 

        /// <summary>
        /// Get Token for Application.
        /// </summary>
        /// <returns>Token for application.</returns>
        public static string GetTokenForApplication()
        {
            var authenticationUrl = "https://login.windows.net/" + ConfigurationManager.AppSettings["TenantName"];
            AuthenticationContext authenticationContext = new AuthenticationContext(authenticationUrl, false);

            // Config for OAuth client credentials 
            ClientCredential clientCred = new ClientCredential(
                ConfigurationManager.AppSettings["AzureADClientId"],
                ConfigurationManager.AppSettings["AzureADClientSecret"]);

            AuthenticationResult authenticationResult = authenticationContext.AcquireToken(ResourceUrl,
                clientCred);
            string token = authenticationResult.AccessToken;
            return token;
        }
    }
}
