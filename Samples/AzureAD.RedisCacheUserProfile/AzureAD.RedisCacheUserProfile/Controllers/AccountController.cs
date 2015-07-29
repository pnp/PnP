using AzureAD.RedisCacheUserProfile.Utils;
using Microsoft.IdentityModel.Clients.ActiveDirectory;
using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace AzureAD.RedisCacheUserProfile.Controllers
{
    public class AccountController : Controller
    {
        public void SignIn()
        {
            if (!Request.IsAuthenticated)
            {
                HttpContext.GetOwinContext().Authentication.Challenge(
                                new AuthenticationProperties { RedirectUri = "/" },
                                OpenIdConnectAuthenticationDefaults.AuthenticationType);
            }
        }

        public void SignOut()
        {
            // Remove all cache entries for this user and send an OpenID Connect sign-out request.
            string usrObjectId = ClaimsPrincipal.Current.FindFirst(SettingsHelper.ClaimTypeObjectIdentifier).Value;
            AuthenticationContext authContext = new AuthenticationContext(SettingsHelper.AzureADAuthority, new EfAdalTokenCache(usrObjectId));
            authContext.TokenCache.Clear();

            HttpContext.GetOwinContext().Authentication.SignOut(
                OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
        }

        public ActionResult ConsentApp()
        {
            string strResource = Request.QueryString["resource"];
            string strRedirectController = Request.QueryString["redirect"];

            string authorizationRequest = String.Format(
                "{0}oauth2/authorize?response_type=code&client_id={1}&resource={2}&redirect_uri={3}",
                    Uri.EscapeDataString(SettingsHelper.AzureADAuthority),
                    Uri.EscapeDataString(SettingsHelper.ClientId),
                    Uri.EscapeDataString(strResource),
                    Uri.EscapeDataString(String.Format("{0}/{1}", this.Request.Url.GetLeftPart(UriPartial.Authority), strRedirectController))
                    );

            return new RedirectResult(authorizationRequest);
        }

        public ActionResult AdminConsentApp()
        {
            string strResource = Request.QueryString["resource"];
            string strRedirectController = Request.QueryString["redirect"];

            string authorizationRequest = String.Format(
                "{0}oauth2/authorize?response_type=code&client_id={1}&resource={2}&redirect_uri={3}&prompt={4}",
                    Uri.EscapeDataString(SettingsHelper.AzureADAuthority),
                    Uri.EscapeDataString(SettingsHelper.ClientId),
                    Uri.EscapeDataString(strResource),
                    Uri.EscapeDataString(String.Format("{0}/{1}", this.Request.Url.GetLeftPart(UriPartial.Authority), strRedirectController)),
                    Uri.EscapeDataString("admin_consent")
                    );

            return new RedirectResult(authorizationRequest);
        }

        public void RefreshSession()
        {
            string strRedirectController = Request.QueryString["redirect"];

            HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = String.Format("/{0}", strRedirectController) }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
        }
    }
}