using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using OfficeDevPnP.MSGraphAPIGroups.Models;
using OfficeDevPnP.MSGraphAPIGroups.Utils;
using Owin;
using System;
using System.IdentityModel.Claims;
using System.Threading.Tasks;
using System.Web;

[assembly: OwinStartup(typeof(OfficeDevPnP.MSGraphAPIGroups.Startup))]

namespace OfficeDevPnP.MSGraphAPIGroups
{
	public class Startup
	{
		public void Configuration(IAppBuilder app)
		{
			ConfigureAuth(app);
		}

		public void ConfigureAuth(IAppBuilder app)
		{
			app.SetDefaultSignInAsAuthenticationType(CookieAuthenticationDefaults.AuthenticationType);

			app.UseCookieAuthentication(new CookieAuthenticationOptions());


			app.UseOpenIdConnectAuthentication(
				new OpenIdConnectAuthenticationOptions
				{
					ClientId = SettingsHelper.ClientId,
					Authority = SettingsHelper.Authority,

					Notifications = new OpenIdConnectAuthenticationNotifications()
					{
						// If there is a code in the OpenID Connect response, redeem it for an access token and refresh token, and store those away.
						AuthorizationCodeReceived = (context) =>
						{
							var code = context.Code;
							ClientCredential credential = new ClientCredential(SettingsHelper.ClientId, SettingsHelper.AppKey);
							String signInUserId = context.AuthenticationTicket.Identity.FindFirst(ClaimTypes.NameIdentifier).Value;

							AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.Authority, new ADALTokenCache(signInUserId));
							AuthenticationResult result = authContext.AcquireTokenByAuthorizationCode(
																							code, 
																							new Uri(HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Path)), 
																							credential, SettingsHelper.MSGraphResource);

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
