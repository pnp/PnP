using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.Configuration;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Linq;
using System;

namespace OutlookNotificationsAPI.WebAPI.Models
{
    public static class TokenHelper
    {
        private static ApplicationDbContext _dbContext = new ApplicationDbContext();

        public static string ClientId
        {
            get { return ConfigurationManager.AppSettings["ida:ClientId"]; }
        }

        public static string AppKey
        {
            get { return ConfigurationManager.AppSettings["ida:ClientSecret"]; }
        }

        public static string AADInstance
        {
            get { return ConfigurationManager.AppSettings["ida:AADInstance"]; }
        }

        public static string TenantId
        {
            get { return ConfigurationManager.AppSettings["ida:TenantId"]; }
        }

        public static string PostLogoutRedirectUri
        {
            get { return ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"]; }
        }

        public static string Authority
        {
            get { return AADInstance + TenantId; }
        }

        public static string OutlookResourceID
        {
            get { return "https://outlook.office.com/"; }
        }

        public static string GraphResourceID
        {
            get { return "https://graph.windows.net/"; }
        }

        public static async Task<string> GetTokenForApplicationAsync(string resource)
        {
            string signedInUserID = ClaimsPrincipal.Current.FindFirst(ClaimTypes.NameIdentifier).Value;
            string tenantID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;
            string userObjectID = ClaimsPrincipal.Current.FindFirst("http://schemas.microsoft.com/identity/claims/objectidentifier").Value;
            return await GetTokenForApplicationAsync(signedInUserID, tenantID, userObjectID, resource);
        }

        public static async Task<string> GetTokenForApplicationAsync(string signedInUserID, 
            string tenantID, string userObjectID, string resource)
        {
            // Get a token for the Graph without triggering any user 
            // interaction (from the cache, via multi-resource refresh token, etc.).
            ClientCredential clientcred = new ClientCredential(ClientId, AppKey);

            // Initialize AuthenticationContext with the token cache of 
            // the currently signed in user, as kept in the app's database.
            AuthenticationContext authenticationContext = new AuthenticationContext(AADInstance + tenantID,
                new ADALTokenCache(signedInUserID));
            AuthenticationResult authenticationResult = await authenticationContext.AcquireTokenSilentAsync(resource,
                clientcred, new UserIdentifier(userObjectID, UserIdentifierType.UniqueId));
            return authenticationResult.AccessToken;
        }
    }
}
