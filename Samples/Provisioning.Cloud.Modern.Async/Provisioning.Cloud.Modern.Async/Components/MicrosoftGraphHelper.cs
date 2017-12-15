using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Claims;
using System.Web;
using Microsoft.Owin.Security.OpenIdConnect;
using Newtonsoft.Json;
using System.Text;
using Newtonsoft.Json.Serialization;
using System.Configuration;

namespace Provisioning.Cloud.Modern.Async.Components
{
    public static class MSGraphAPISettings
    {
        public static string ClientId { get { return ConfigurationManager.AppSettings["ida:ClientId"]; } }
        public static string ClientSecret { get { return ConfigurationManager.AppSettings["ida:ClientSecret"]; } }
        public static string AADInstance { get { return ConfigurationManager.AppSettings["ida:AADInstance"]; } }

        public static string MicrosoftGraphResourceId = "https://graph.microsoft.com";
    }

    public static class MicrosoftGraphHelper
    {
        public static String MicrosoftGraphV1BaseUri = "https://graph.microsoft.com/v1.0/";
        public static String MicrosoftGraphBetaBaseUri = "https://graph.microsoft.com/beta/";

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
                resourceId = MSGraphAPISettings.MicrosoftGraphResourceId;
            }

            try
            {
                ClientCredential credential = new ClientCredential(
                    MSGraphAPISettings.ClientId,
                    MSGraphAPISettings.ClientSecret);
                string signedInUserID = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    ClaimTypes.NameIdentifier).Value;
                string tenantId = System.Security.Claims.ClaimsPrincipal.Current.FindFirst(
                    "http://schemas.microsoft.com/identity/claims/tenantid").Value;
                AuthenticationContext authContext = new AuthenticationContext(
                    MSGraphAPISettings.AADInstance + tenantId,
                    new SessionADALCache(signedInUserID));

                AuthenticationResult result = authContext.AcquireTokenSilent(
                    resourceId,
                    credential,
                    UserIdentifier.AnyUser);

                accessToken = result.AccessToken;
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
