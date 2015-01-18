using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AzureAD.GroupMembership
{
    public static class AzureAdAuthentication
    {
        private const string AzureAdTenantLoginUrl = "https://login.windows.net/";
        private const string GraphApiUrl = "https://graph.windows.net";

        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication(Uri sharePointAdminUrl)
        {
            Uri servicePointUri = new Uri("");
            string adminRealm = "";
            Uri serviceRoot = new Uri(servicePointUri, adminRealm);

            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(serviceRoot,
                async () => await AcquireApplicationTokenAsync());
            return activeDirectoryClient;
        } 

        public static async Task<string> AcquireApplicationTokenAsync()
        {
            var authenticationUrl = AzureAdTenantLoginUrl + ConfigurationManager.AppSettings["TenantUpnDomain"];
            AuthenticationContext authenticationContext = new AuthenticationContext(authenticationUrl, false);

            // Config for OAuth client credentials 
            ClientCredential clientCred = new ClientCredential(
                ConfigurationManager.AppSettings["ClientId"],
                ConfigurationManager.AppSettings["ClientSecret"]);

            AuthenticationResult authenticationResult = authenticationContext.AcquireToken(
                GraphApiUrl,
                clientCred);

            string token = authenticationResult.AccessToken;
            return token;
        }
    }
}
