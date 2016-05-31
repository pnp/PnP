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
using MicrosoftGraph.Office365.DotNetSDK.WebApp.Models;
using MicrosoftGraph.Office365.DotNetSDK.WebApp.Components;

namespace MicrosoftGraph.Office365.DotNetSDK.WebApp
{
    public partial class Startup
    {

        public static readonly string Authority = ADALHelper.AADInstance + ADALHelper.TenantId;

        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = ADALHelper.ClientId,
                    Authority = Authority,
                    PostLogoutRedirectUri = ADALHelper.PostLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                       AuthorizationCodeReceived = (context) => 
                       {
                           var code = context.Code;

                           ClientCredential credential = new ClientCredential(
                               ADALHelper.ClientId, 
                               ADALHelper.ClientSecret);
                           string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(
                               ClaimTypes.NameIdentifier).Value;
                           string tenantId = context.AuthenticationTicket.Identity.FindFirst(
                               "http://schemas.microsoft.com/identity/claims/tenantid").Value;

                           AuthenticationContext authContext = new AuthenticationContext(
                               Authority, new SessionADALCache(signedInUserID));

                           AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                               code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), 
                               credential, ADALHelper.MicrosoftGraphResourceId);

                           return Task.FromResult(0);
                       }
                    }
                });
        }
    }
}
