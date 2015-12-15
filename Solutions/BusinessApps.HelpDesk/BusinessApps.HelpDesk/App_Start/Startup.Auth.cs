using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Owin;
using BusinessApps.HelpDesk.Models;
using BusinessApps.HelpDesk.Helpers;

namespace BusinessApps.HelpDesk
{
    public partial class Startup
    {
        public void ConfigureAuth(IAppBuilder app)
        {
            ApplicationDbContext db = new ApplicationDbContext();

            app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

            app.UseCookieAuthentication(new CookieAuthenticationOptions());

            OpenIdConnectAuthenticationOptions options = new OpenIdConnectAuthenticationOptions();
            options.ClientId = SettingsHelper.ClientId;
            options.Authority = SettingsHelper.AzureADAuthority;
            options.PostLogoutRedirectUri = SettingsHelper.LogoutAuthority;

            OpenIdConnectAuthenticationNotifications notifications = new OpenIdConnectAuthenticationNotifications();
            notifications.AuthorizationCodeReceived = (context) =>
            {
                string code = context.Code;

                AuthenticationHelper authHelper = new AuthenticationHelper();
                
                String signInUserId = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

                AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthAuthority);
                AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(code, new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), authHelper.GetClientAssertionCertificate(), null);

                return Task.FromResult(0);
            };

            notifications.RedirectToIdentityProvider = (context) =>
            {
                string appBaseUrl = context.Request.Scheme + "://" + context.Request.Host + context.Request.PathBase;
                context.ProtocolMessage.RedirectUri = appBaseUrl + "/";
                context.ProtocolMessage.PostLogoutRedirectUri = appBaseUrl;

                return Task.FromResult(0);
            };

            notifications.AuthenticationFailed = (context) =>
            {
                context.HandleResponse();
                return Task.FromResult(0);
            };

            options.Notifications = notifications;

            app.UseOpenIdConnectAuthentication(options);
        }
    }
}
