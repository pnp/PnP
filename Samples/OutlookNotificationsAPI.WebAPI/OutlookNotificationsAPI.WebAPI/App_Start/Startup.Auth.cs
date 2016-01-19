using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OutlookNotificationsAPI.WebAPI.Models;
using Owin;
using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;

namespace OutlookNotificationsAPI.WebAPI
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            // Use the cookie manager described here http://katanaproject.codeplex.com/wikipage?title=System.Web%20response%20cookie%20integration%20issues&referringTitle=Documentation
            //app.UseCookieAuthentication(new CookieAuthenticationOptions());
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                CookieManager = new SystemWebCookieManager()
            });

            app.UseOpenIdConnectAuthentication(
                new OpenIdConnectAuthenticationOptions
                {
                    ClientId = TokenHelper.ClientId,
                    Authority = TokenHelper.Authority,
                    PostLogoutRedirectUri = TokenHelper.PostLogoutRedirectUri,

                    Notifications = new OpenIdConnectAuthenticationNotifications()
                    {
                        // If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
                       AuthorizationCodeReceived = (context) => 
                       {
                           var code = context.Code;
                           ClientCredential credential = new ClientCredential(TokenHelper.ClientId, TokenHelper.AppKey);
                           string signedInUserID = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;
                           AuthenticationContext authContext = new AuthenticationContext(TokenHelper.Authority, new ADALTokenCache(signedInUserID));
                           AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
                           code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), credential, TokenHelper.GraphResourceID);

                           return Task.FromResult(0);
                       }
                    }
                });
        }
    }
}
