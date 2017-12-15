using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.Linq;
using System.Web;
using Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Threading.Tasks;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using System.IdentityModel.Claims;
using Provisioning.Cloud.Modern.Async.Components;

namespace Provisioning.Cloud.Modern.Async
{
    public partial class Startup
    {
        private static string clientId = ConfigurationManager.AppSettings["ida:ClientId"];
        private static string clientSecret = ConfigurationManager.AppSettings["ida:ClientSecret"];
        private static string aadInstance = ConfigurationManager.AppSettings["ida:AADInstance"];
        private static string tenantId = ConfigurationManager.AppSettings["ida:TenantId"];
        private static string postLogoutRedirectUri = ConfigurationManager.AppSettings["ida:PostLogoutRedirectUri"];
        private static string authority = aadInstance + tenantId;

        public void ConfigureAuth(IAppBuilder app)
        {
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = clientId,
                    Authority = authority,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        SecurityTokenValidated = (context) =>
                        {
                            return Task.FromResult(0);
                        },
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;

                            ClientCredential credential = new ClientCredential(
                                clientId,
                                clientSecret);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(
                                ClaimTypes.NameIdentifier).Value;
                            string tenantId = context.AuthenticationTicket.Identity.FindFirst(
                                "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                            AuthenticationContext authContext = new AuthenticationContext(
                                aadInstance + tenantId,
                                new SessionADALCache(signedInUserID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                                code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                credential, MSGraphAPISettings.MicrosoftGraphResourceId);

                            return Task.FromResult(0);
                        },
                        AuthenticationFailed = (context) =>
                        {
                            context.OwinContext.Response.Redirect("/Home/Error?message=" + context.Exception.Message);
                            context.HandleResponse(); // Suppress the exception
                            return Task.FromResult(0);
                        }
                    }
                });
        }
    }
}
