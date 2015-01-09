using Microsoft.Office365.OAuth;
using Microsoft.Azure.ActiveDirectory.GraphClient;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class ActiveDirectoryApiSample
    {
        public static async Task<IEnumerable<IUser>> GetUsers()
        {
            var client = EnsureClientCreated();

            var userResults = await client.DirectoryObjects.OfType<User>().ExecuteAsync();

            List<IUser> allUsers = new List<IUser>();

            do
            {
                allUsers.AddRange(userResults.CurrentPage);
                userResults = await userResults.GetNextPageAsync();
            } while (userResults != null);

            return allUsers;
        }

        public static ActiveDirectoryClient EnsureClientCreated()
        {
            Uri serviceRoot = new Uri(
                new Uri(Office365ServicesUris.AADGraphAPIResourceId), 
                AuthenticationHelper.AuthenticationResult.TenantId);

            // Create the ActiveDirectoryClient client proxy:
            return new ActiveDirectoryClient(
                serviceRoot,
                async () =>
                {
                    return await AuthenticationHelper.GetAccessTokenForServiceAsync(Office365ServicesUris.AADGraphAPIResourceId);
                });
        }
    }
}
