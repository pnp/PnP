using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Demo
{
    public static class AuthenticationHelper
    {
        public static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientID"].ToString();
        public static readonly string RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"].ToString();

        public static AuthenticationContext AuthenticationContext
        {
            get;
            private set;
        }

        public static AuthenticationResult AuthenticationResult
        {
            get;
            private set;
        }

        public static void Authenticate(String authority)
        {
            if (AuthenticationHelper.AuthenticationContext == null)
            {
                AuthenticationHelper.AuthenticationContext = new AuthenticationContext(authority);

                var tokenCache = AuthenticationContext.TokenCache.ReadItems().FirstOrDefault();

                if (tokenCache != null)
                {
                    AuthenticationHelper.AuthenticationContext = new AuthenticationContext(tokenCache.Authority);
                }
            }

            AuthenticationHelper.AuthenticationResult =
                AuthenticationHelper.AuthenticationContext.AcquireToken(
                    Office365ServicesUris.AADGraphAPIResourceId, 
                    ClientId, 
                    new Uri(RedirectUri));
        }

        public static async Task<String> GetAccessTokenForServiceAsync(CapabilityDiscoveryResult discoveryCapabilityResult)
        {
            return await GetAccessTokenForServiceAsync(discoveryCapabilityResult.ServiceResourceId);
        }

        public static async Task<String> GetAccessTokenForServiceAsync(String serviceResourceId)
        {
            var authResult = await AuthenticationHelper.AuthenticationContext.AcquireTokenSilentAsync(serviceResourceId, ClientId);

            return authResult.AccessToken;
        }
    }
}
