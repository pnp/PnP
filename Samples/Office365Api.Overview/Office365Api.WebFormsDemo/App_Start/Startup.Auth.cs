using System;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.DataProtection;
using Microsoft.Owin.Security.Google;
using Owin;
using Office365Api.WebFormsDemo.Models;

using Microsoft.Owin.Security;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Office365Api.Helpers;
using System.IdentityModel.Claims;
using System.Web;

namespace Office365Api.WebFormsDemo
{
    public partial class Startup {

        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301883
        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = AuthenticationHelper.ClientId,
                    Authority = AuthenticationHelper.AuthorityMultitenant,

                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        ValidateIssuer = false
                    },

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        //
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.

                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;

                            ClientCredential credential = new ClientCredential(AuthenticationHelper.ClientId, AuthenticationHelper.SharedSecret);
                            String userObjectId = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                            String tenantId = context.AuthenticationTicket.Identity.FindFirst("http://schemas.microsoft.com/identity/claims/tenantid").Value;

                            AuthenticationContext authContext = new AuthenticationContext(String.Format("{0}/{1}", AuthenticationHelper.AuthorizationUri, tenantId), new ADALTokenCache(userObjectId));

                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, Office365ServicesUris.AADGraphAPIResourceId);

                            return Task.FromResult(0);
                        },


                        RedirectToIdentityProvider = (context) =>
                        {
                            // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                            // this allows you to deploy your app (to Azure Web Sites, for example)without having to change settings
                            // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                            string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                            context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                            context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                            return Task.FromResult(0);
                        },

                        AuthenticationFailed = (context) =>
                        {
                            // Suppress the exception if you don't want to see the error
                            context.HandleResponse();

                            return Task.FromResult(0);
                        }

                    }

                });
        }
    }
}
