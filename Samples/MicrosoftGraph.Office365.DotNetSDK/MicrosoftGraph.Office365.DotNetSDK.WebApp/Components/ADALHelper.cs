using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IdentityModel.Claims;
using System.Linq;
using System.Web;

namespace MicrosoftGraph.Office365.DotNetSDK.WebApp.Components
{
    public static class ADALHelper
    {
        public const String MicrosoftGraphResourceId = "https://graph.microsoft.com/";
        public static String ClientId = ConfigurationManager.AppSettings["ida:ClientID"];
        public static String ClientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        public static String AADInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        public static String TenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        public static String PostLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];

        /// <summary>
        /// This helper method returns and OAuth Access Token for the current user
        /// </summary>
        /// <param name="resourceId">The resourceId for which we are requesting the token</param>
        /// <returns>The OAuth Access Token value</returns>
        public static String GetAccessTokenForCurrentUser(String resourceId = null)
        {
            String accessToken = null;

            if (String.IsNullOrEmpty(resourceId))
            {
                resourceId = MicrosoftGraphResourceId;
            }

            try
            {
                ClientCredential credential = new ClientCredential(
                    ClientId,
                    ClientSecret);

                String signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    ClaimTypes.NameIdentifier).Value;
                String tenantId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                AuthenticationContext authContext = new AuthenticationContext(
                    AADInstance + TenantId,
                    new SessionADALCache(signedInUserID));

                AuthenticationResult result = authContext.AcquireTokenSilent(
                    resourceId,
                    credential,
                    UserIdentifier.AnyUser);

                if (result != null)
                {
                    accessToken = result.AccessToken;
                }
            }
            catch (AdalException ex)
            {
                if (ex.ErrorCode == "failed_to_acquire_token_silently")
                {
                    // Refresh the access token from scratch
                    HttpContext.Current.GetOwinContext().Authentication.Challenge(
                        new AuthenticationProperties
                        {
                            RedirectUri = HttpContext.Current.Request.Url.ToString(),
                        },
                        OpenIdConnectAuthenticationDefaults.AuthenticationType);
                }
                else
                {
                    // Rethrow the exception
                    throw ex;
                }
            }

            return (accessToken);
        }
    }
}