using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Office365.Discovery;
using Microsoft.Office365.OutlookServices;
using Microsoft.Office365.SharePoint.CoreServices;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Office365Api.Helpers
{
    public class AuthenticationHelper
    {
        public static readonly string AuthorizationUri = ConfigurationManager.AppSettings["ida:AuthorizationUri"].ToString();
        public static readonly string ClientId = ConfigurationManager.AppSettings["ida:ClientId"].ToString();

        public static readonly string SharedSecret = ConfigurationManager.AppSettings["ida:AppKey"] ?? ConfigurationManager.AppSettings["ida:Password"];
        public static readonly string RedirectUri = ConfigurationManager.AppSettings["ida:RedirectUri"] != null ? ConfigurationManager.AppSettings["ida:RedirectUri"].ToString() : null;
        public static readonly string AuthorityMultitenant = String.Format("{0}/common", AuthenticationHelper.AuthorizationUri);

        public AuthenticationContext AuthenticationContext
        {
            get;
            private set;
        }

        public AuthenticationResult AuthenticationResult
        {
            get;
            private set;
        }

        public void EnsureAuthenticationContext(String authority)
        {
            if (this.AuthenticationContext == null)
            {
                this.AuthenticationContext = new AuthenticationContext(authority);

                var tokenCacheItem = AuthenticationContext.TokenCache.ReadItems().FirstOrDefault();

                if (tokenCacheItem != null)
                {
                    this.AuthenticationContext = new AuthenticationContext(tokenCacheItem.Authority);
                }
            }

            this.AuthenticationResult =
                this.AuthenticationContext.AcquireToken(
                    Office365ServicesUris.AADGraphAPIResourceId,
                    ClientId,
                    new Uri(RedirectUri));
        }

        public void EnsureAuthenticationContext(TokenCache tokenCache)
        {
            if (ClaimsPrincipal.Current != null)
            {
                var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                var tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                this.AuthenticationContext = new AuthenticationContext(
                    String.Format("{0}/{1}", AuthenticationHelper.AuthorizationUri, tenantId),
                    tokenCache);
            }
        }

        public async Task<String> GetAccessTokenForServiceAsync(String serviceResourceId)
        {
            if (this.AuthenticationContext == null)
                throw new NullReferenceException("Please, initiliaze the AuthenticationContext by calling the EnsureAuthenticationContext method!");

            // If the caller is NOT a web application
            if (System.Web.HttpContext.Current == null)
            {
                var authResult = await this.AuthenticationContext.AcquireTokenSilentAsync(serviceResourceId, ClientId);

                return (authResult.AccessToken);
            }
            else if (ClaimsPrincipal.Current != null)
            {
                var signInUserId = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
                var userObjectId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
                var tenantId = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                var authResult = await this.AuthenticationContext.AcquireTokenSilentAsync(serviceResourceId,
                                                                           new ClientCredential(AuthenticationHelper.ClientId,
                                                                                                AuthenticationHelper.SharedSecret),
                                                                           new UserIdentifier(userObjectId,
                                                                                              UserIdentifierType.UniqueId));

                return (authResult.AccessToken);
            }
            else
            {
                return (null);
            }
        }

        public async Task<OutlookServicesClient> EnsureOutlookServicesClientCreatedAsync(string capabilityName)
        {
            try
            {
                DiscoveryClient discoveryClient = new DiscoveryClient(
                    Office365ServicesUris.DiscoveryServiceEndpointUri,
                    async () => { return await GetAccessTokenForServiceAsync(Office365ServicesUris.DiscoveryServiceResourceId); });

                var dcr = await discoveryClient.DiscoverCapabilityAsync(capabilityName);

                return new OutlookServicesClient(dcr.ServiceEndpointUri,
                    async () => { return await GetAccessTokenForServiceAsync(dcr.ServiceResourceId); });
            }
            catch (AdalException exception)
            {
                // Handle token acquisition failure
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    this.AuthenticationContext.TokenCache.Clear();
                    throw exception;
                }
                return null;
            }
        }

        public async Task<SharePointClient> EnsureSharePointClientCreatedAsync(string capabilityName)
        {
            try
            {
                DiscoveryClient discoveryClient = new DiscoveryClient(
                    Office365ServicesUris.DiscoveryServiceEndpointUri,
                    async () => { return await GetAccessTokenForServiceAsync(Office365ServicesUris.DiscoveryServiceResourceId); });

                var dcr = await discoveryClient.DiscoverCapabilityAsync(capabilityName);

                return new SharePointClient(dcr.ServiceEndpointUri,
                    async () => { return await GetAccessTokenForServiceAsync(dcr.ServiceResourceId); });
            }
            catch (AdalException exception)
            {
                // Handle token acquisition failure
                if (exception.ErrorCode == AdalError.FailedToAcquireTokenSilently)
                {
                    this.AuthenticationContext.TokenCache.Clear();
                    throw exception;
                }
                return null;
            }
        }
    }
}
