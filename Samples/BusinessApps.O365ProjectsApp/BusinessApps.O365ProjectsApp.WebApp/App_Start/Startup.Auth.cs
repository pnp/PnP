using System;
using System.Collections.Generic;
using System.Configuration;
using System.Globalization;
using System.IdentityModel.Claims;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using BusinessApps.O365ProjectsApp.WebApp.Models;
using BusinessApps.O365ProjectsApp.Infrastructure;

namespace BusinessApps.O365ProjectsApp.WebApp
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = O365ProjectsAppSettings.ClientId,
                    Authority = O365ProjectsAppSettings.AADInstance + "common",
                    PostLogoutRedirectUri = O365ProjectsAppSettings.PostLogoutRedirectUri,
                    TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                    {
                        // instead of using the default validation (validating against a single issuer value, as we do in line of business apps), 
                        // we inject our own multitenant validation logic
                        ValidateIssuer = false,
                    },
                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                        AuthorizationCodeReceived = (context) =>
                        {
                            var code = context.Code;

                            ClientCredential credential = new ClientCredential(
                                O365ProjectsAppSettings.ClientId,
                                O365ProjectsAppSettings.ClientSecret);
                            string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(
                                ClaimTypes.NameIdentifier).Value;
                            string tenantId = context.AuthenticationTicket.Identity.FindFirst(
                                "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                            AuthenticationContext authContext = new AuthenticationContext(
                                O365ProjectsAppSettings.AADInstance + tenantId,
                                new SessionADALCache(signedInUserID));
                            AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                                code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)),
                                credential, O365ProjectsAppSettings.MicrosoftGraphResourceId);

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
