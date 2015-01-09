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
                ClientCredential clientCred = new ClientCredential(
                    ConfigurationManager.AppSettings["AzureADClientId"],
                    ConfigurationManager.AppSettings["AzureADClientSecret"]);
                string resource = "https://graph.windows.net";
                string token = String.Empty;

                // Authenticate
                AuthenticationResult authenticationResult = authenticationContext.AcquireToken(resource, clientCred);
                token = authenticationResult.AccessToken;

                var activeDirectoryClient = AuthenticationHelper.GetActiveDirectoryClientAsApplication();

                List<IUser> users = activeDirectoryClient.Users.ExecuteAsync().Result.CurrentPage.ToList();

                foreach (var user in users)
                {
                    Console.WriteLine(user.DisplayName);
                }
            }
            catch (AuthenticationException ex)
            {

            }
            Console.ReadLine();

        }
    }
}
