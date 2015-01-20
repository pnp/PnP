using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace AzureAD.GroupMembership
{
    public static class AzureAdAuthentication
    {
        private const string AzureAdTenantLoginUrl = "https://login.windows.net/";
        private const string AzureAdFederationUrl = "https://login.windows.net/{0}/FederationMetadata/2007-06/FederationMetadata.xml";
        private const string GraphApiUrl = "https://graph.windows.net/";

        public static string GetTenantId()
        {
            string url = string.Format(AzureAdFederationUrl, ConfigurationManager.AppSettings["TenantUpnDomain"]);
            var document = XDocument.Load(url);
            var stsUri = document.Root.Attribute("entityID");
            string tenantId = stsUri.Value.Substring(24, 36); //this line removes sts.windows.net
            return tenantId;
        }

        public static ActiveDirectoryClient GetActiveDirectoryClientAsApplication()
        {
            var tenantId = GetTenantId();
            Uri authenticationUri = new Uri(GraphApiUrl + tenantId);

            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(authenticationUri, async () => await AcquireApplicationTokenAsync());
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

            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(
                GraphApiUrl,
                clientCred);

            string token = authenticationResult.AccessToken;
            return token;
        }
    }
}
