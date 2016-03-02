using Microsoft.Owin.Security;
using Microsoft.Owin.Security.Cookies;
using Microsoft.Owin.Security.OpenIdConnect;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIGroups.Controllers
{
	public class AccountController : Controller
	{
		public void SignIn()
		{
			if (!Request.IsAuthenticated)
			{
				HttpContext.GetOwinContext().Authentication.Challenge(new AuthenticationProperties { RedirectUri = "/" }, OpenIdConnectAuthenticationDefaults.AuthenticationType);
			}
		}
		public void SignOut()
		{
			string callbackUrl = Url.Action("SignOutCallback", "Account", routeValues: null, protocol: Request.Url.Scheme);

			HttpContext.GetOwinContext().Authentication.SignOut(
					new AuthenticationProperties { RedirectUri = callbackUrl },
					OpenIdConnectAuthenticationDefaults.AuthenticationType, CookieAuthenticationDefaults.AuthenticationType);
		}

		public ActionResult SignOutCallback()
		{
			if (Request.IsAuthenticated)
			{
				// Redirect to home page if the user is authenticated.
				return RedirectToAction("Index", "Home");
			}

			return View();
		}
	}
}