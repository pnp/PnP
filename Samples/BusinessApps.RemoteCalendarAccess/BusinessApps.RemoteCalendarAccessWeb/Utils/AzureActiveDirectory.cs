using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.Azure.ActiveDirectory.GraphClient.Extensions;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace BusinessApps.RemoteCalendarAccessWeb.Utils
{
    public class AzureActiveDirectory
    {
        private string _accessToken;
        private ActiveDirectoryClient _client;

        public AzureActiveDirectory()
        {
            _client = GetClient();
        }

        private async Task<String> GetAccessToken()
        {
            if (string.IsNullOrEmpty(_accessToken))
            {
                string url = ConfigurationManager.AppSettings["AzureGraphAuthURL"];
                string appId = ConfigurationManager.AppSettings["AzureID"];
                string appSecret = ConfigurationManager.AppSettings["AzureSecret"];
                string serviceRealm = ConfigurationManager.AppSettings["AzureServiceRealm"];

                var context = new AuthenticationContext(url);

                var credential = new ClientCredential(appId, appSecret);

                var token = await context.AcquireTokenAsync(serviceRealm, credential);
                _accessToken = token.AccessToken;
            }

            return _accessToken;
        }

        private ActiveDirectoryClient GetClient()
        {
            Uri baseServiceUri = new Uri(ConfigurationManager.AppSettings["AzureGraphURL"]);
            ActiveDirectoryClient activeDirectoryClient = new ActiveDirectoryClient(new Uri(baseServiceUri, ConfigurationManager.AppSettings["O365Domain"]), 
                                                                                    async () => { return await GetAccessToken(); });
            return activeDirectoryClient;
        }

        public async Task<IUser> GetUser(string userPrincipalName)
        {
            IPagedCollection<IUser> users = await _client.Users.Where(u => u.UserPrincipalName == userPrincipalName).ExecuteAsync().ConfigureAwait(false);
            if (!users.CurrentPage.Any())
                throw new Exception("User " + userPrincipalName + " not found.");

            return users.CurrentPage.First();
        }
    }
}