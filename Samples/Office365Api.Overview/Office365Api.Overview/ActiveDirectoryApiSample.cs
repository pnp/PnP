using Microsoft.Office365.OAuth;
using Microsoft.Office365.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Overview
{
    public static class ActiveDirectoryApiSample
    {
        const string AadGraphResource = "https://graph.windows.net/";

        public static async Task<IEnumerable<IUser>> GetUsers()
        {
            var client = await EnsureClientCreated();

            var userResults = await client.DirectoryObjects.OfType<User>().ExecuteAsync();

            List<IUser> allUsers = new List<IUser>();

            do
            {
                allUsers.AddRange(userResults.CurrentPage);
                userResults = await userResults.GetNextPageAsync();
            } while (userResults != null);

            return allUsers;
        }

        private static async Task<AadGraphClient> EnsureClientCreated()
        {
            Authenticator authenticator = new Authenticator();
            var authInfo = await authenticator.AuthenticateAsync(AadGraphResource);

            return new AadGraphClient(new Uri(AadGraphResource + authInfo.IdToken.TenantId), authInfo.GetAccessToken);
        }
        public static async Task SignOut()
        {
            await new Authenticator().LogoutAsync();
        }
    }
}
