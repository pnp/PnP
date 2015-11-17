using AzureAD.RedisCacheUserProfile.Utils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Owin;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace AzureAD.RedisCacheUserProfile
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // configure the authentication type & settings
            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);
            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            // configure the OWIN OpenId Connect options
            app.UseOpenIdConnectAuthentication(new OpenIdConnectAuthenticationOptions
            {
                ClientId = SettingsHelper.ClientId,
                Authority = SettingsHelper.AzureADAuthority,
                TokenValidationParameters = new System.IdentityModel.Tokens.TokenValidationParameters
                {
                    // we inject our own multitenant validation logic
                    ValidateIssuer = false,
                    // map the claimsPrincipal's roles to the roles claim
                    RoleClaimType = "roles",
                },
                Notifications = new OpenIdConnectAuthenticationNotifications()
                {
                    RedirectToIdentityProvider = (context) =>
                    {
                        // This ensures that the address used for sign in and sign out is picked up dynamically from the request
                        // this allows you to deploy your app (to Azure Web Sites, for example) without having to change settings
                        // Remember that the base URL of the address used here must be provisioned in Azure AD beforehand.
                        //string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                        context.ProtocolMessage.RedirectUri = HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path);
                        context.ProtocolMessage.PostLogoutRedirectUri = new UrlHelper(HttpContext.Current.Request.RequestContext).Action("Index", "Home", null, HttpContext.Current.Request.Url.Scheme);
                        context.ProtocolMessage.Resource = SettingsHelper.GraphResourceId;

                        return Task.FromResult(0);
                    },

                    // when an auth code is received...
                    AuthorizationCodeReceived = (context) =>
                    {
                        // get the OpenID Connect code passed from Azure AD on successful auth
                        string code = context.Code;

                        // create the app credentials & get reference to the user
                        ClientCredential creds = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.ClientSecret);
                        string userObjectId = context.AuthenticationTicket.Identity.FindFirst(System.IdentityModel.Claims.ClaimTypes.NameIdentifier).Value;

                        // use the ADAL to obtain access token & refresh token...
                        //  save those in a persistent store...
                        EfAdalTokenCache sampleCache = new EfAdalTokenCache(userObjectId);
                        AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, sampleCache);

                        // obtain access token for the AzureAD graph
                        Uri redirectUri = new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path));
                        AuthenticationResult authResult = authContext.AcquireTokenByAuthorizationCode(code, redirectUri, creds, SettingsHelper.AzureAdGraphResourceId);

                        if (GraphUtil.IsUserAADAdmin(context.AuthenticationTicket.Identity))
                            context.AuthenticationTicket.Identity.AddClaim(new Claim("roles", "admin"));

                        var groups = GraphUtil.GetMemberGroups(context.AuthenticationTicket.Identity).Result;

                        foreach (string groupid in groups)
                        {
                            var displayname = GraphUtil.LookupDisplayNameOfAADObject(groupid, context.AuthenticationTicket.Identity);
                            context.AuthenticationTicket.Identity.AddClaim(new Claim("roles", displayname));
                        }

                        // successful auth
                        return Task.FromResult(0);
                    },
                    SecurityTokenValidated = (context) =>
                    {

                        return Task.FromResult(0);
                    },
                    AuthenticationFailed = (context) =>
                    {
                        context.HandleResponse();
                        return Task.FromResult(0);
                    }
                }
            });
        }

    }
}