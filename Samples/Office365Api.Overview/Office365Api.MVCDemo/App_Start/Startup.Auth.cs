using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Office365Api.Helpers;
using Owin;
using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;

namespace Office365Api.MVCDemo
{
    public partial class Startup
    {
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
