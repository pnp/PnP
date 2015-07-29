using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;
using System.Web;

namespace AzureAD.RedisCacheUserProfile.Utils
{
     public class AppToken
    {
        public static async Task<string> GetAppTokenAsync()
        {
            string Authority = String.Format(CultureInfo.InvariantCulture, SettingsHelper.AzureADAuthority, SettingsHelper.Tenant);

            // Instantiate an AuthenticationContext for my directory (see authString above).
            AuthenticationContext authenticationContext = new AuthenticationContext(Authority, false);

            // Create a ClientCredential that will be used for authentication.
            // This is where the Client ID and Key/Secret from the Azure Management Portal is used.
            ClientCredential clientCred = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);

            // Acquire an access token from Azure AD to access the Azure AD Graph (the resource)
            // using the Client ID and Key/Secret as credentials.
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenAsync(SettingsHelper.GraphResourceId, clientCred);

            // Return the access token.
            return authenticationResult.AccessToken;
        }

        public static string GetAppToken()
        {
            string Authority = String.Format(CultureInfo.InvariantCulture, SettingsHelper.AzureADAuthority, SettingsHelper.Tenant);

            // Instantiate an AuthenticationContext for my directory (see authString above).
            AuthenticationContext authenticationContext = new AuthenticationContext(Authority, false);

            // Create a ClientCredential that will be used for authentication.
            // This is where the Client ID and Key/Secret from the Azure Management Portal is used.
            ClientCredential clientCred = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);

            // Acquire an access token from Azure AD to access the Azure AD Graph (the resource)
            // using the Client ID and Key/Secret as credentials.
            AuthenticationResult authenticationResult = authenticationContext.AcquireToken(SettingsHelper.GraphResourceId, clientCred);

            // Return the access token.
            return authenticationResult.AccessToken;
        }
    }
}
