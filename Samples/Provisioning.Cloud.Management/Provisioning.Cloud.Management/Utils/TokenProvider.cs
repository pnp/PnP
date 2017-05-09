using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using Provisioning.Cloud.Management.Models;

namespace Provisioning.Cloud.Management.Utils
{
    public class TokenProvider
    {
        /// <summary>
        /// Retrieves an access token for SharePoint in the name of the current user for the given tenancy.
        /// </summary>
        /// <returns>The access token</returns>
        public Task<string> GetSharePointAdminAccessToken()
        {
            return GetAccessToken(SettingsHelper.SharePointAdminResourceUri);
        }

        /// <summary>
        /// Retrieves an access token for the Graph Service in the name of the current user for the given tenancy.
        /// </summary>
        /// <returns>The access token</returns>
        public Task<string> GetGraphServiceAccessToken()
        {
            return GetAccessToken(SettingsHelper.AADGraphResourceId);
        }

        /// <summary>
        /// Retrieves an access token for the Discovery Service in the name of the current user for the given tenancy.
        /// </summary>
        /// <returns>The access token</returns>
        public Task<string> GetDiscoveryServiceAccessToken()
        {
            return GetAccessToken(SettingsHelper.DiscoveryServiceEndpointUri.ToString());
        }

        private static async Task<string> GetAccessToken(string resource)
        {
            // Redeem the authorization code from the response for an access token and refresh token.
            var principal = ClaimsPrincipal.Current;

            var nameIdentifier = principal.FindFirst(ClaimTypes.NameIdentifier).Value;
            var tenantId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

            AuthenticationContext authContext = new AuthenticationContext(
                string.Format("{0}/{1}", SettingsHelper.AuthorizationUri, tenantId),
                new ADALTokenCache(nameIdentifier)
            );

            try
            {
                var objectId = principal.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;

                var result = await authContext.AcquireTokenSilentAsync(
                    resource,
                    new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey),
                    new UserIdentifier(objectId, UserIdentifierType.UniqueId)
                );

                return result.AccessToken;
            }
            catch (AdalException exception)
            {
                //handle token acquisition failure
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    authContext.TokenCache.Clear();
                }
            }

            return null;
        }
    }
}