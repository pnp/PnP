using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace Core.UserProfiles.Sync
{
    class Program
    {
        static void Main(string[] args)
        {
            try
            {
                // get OAuth token using Client Credentials
                string authString = "https://login.windows.net/" + ConfigurationManager.AppSettings["TenantName"];
                AuthenticationContext authenticationContext = new AuthenticationContext(authString, false);

                // Config for OAuth client credentials 
                ClientCredential clientCred = new ClientCredential(ConfigurationManager.AppSettings["AzureADClientId"], ConfigurationManager.AppSettings["AzureADClientSecret"]);
                string resource = "https://graph.windows.net";
                string token = String.Empty;

                // Authenticate
                AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, clientCred);
                token = authenticationResult.AccessToken;

                GraphConnection graphConnection = SetupGraphConnection(token);

            }
            catch (AuthenticationException ex)
            {

            }

        }
        
        private static GraphConnection SetupGraphConnection(string accessToken)
        {
            Guid ClientRequestId = Guid.NewGuid();
            GraphSettings graphSettings = new GraphSettings();
            graphSettings.ApiVersion = "2013-11-08";
            graphSettings.GraphDomainName = "graph.windows.net";
            return new GraphConnection(accessToken, ClientRequestId, graphSettings);
        }

    }
}
