using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.SharePoint.Client;
using Provisioning.CreateSite.MvcWeb.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Provisioning.CreateSite.MvcWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            
            // Get the current user's name so we can add it to the ViewBag and display it on the view.
            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }

        [SharePointContextFilter]
        public ActionResult Scenario1()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var cc = spContext.CreateAppOnlyClientContextForSPHost())
            {
                cc.Load(cc.Web);
                cc.ExecuteQueryRetry();

                // Get all of the WebTemplates so we can pass it to NewWebProperties to populate the SelectListItems
                WebTemplateCollection webTemplates = cc.Web.GetAvailableWebTemplates(cc.Web.Language, false);
                cc.Load(webTemplates);
                cc.ExecuteQueryRetry();

                var props = new NewWebProperties(webTemplates);
                return View(props);
            }            
        }

        [SharePointContextFilter]
        public ActionResult Scenario2()
        {
            User spUser;
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            // Get the current user's email to specify as the Site Collection owner. 
            using (var cc = spContext.CreateUserClientContextForSPHost())
            {
                spUser = cc.Web.CurrentUser;
                cc.Load(spUser);
                cc.ExecuteQueryRetry();
            }
            var model = new NewSiteProperties
            {
                SiteOwnerEmail = spUser.Email
            };
            return View(model);
        }

        [SharePointContextFilter]
        [AcceptVerbs("Get", "Post")]
        public ActionResult ValidateSite(string url)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var baseUrl = $"{spContext.SPHostUrl.Scheme}://{spContext.SPHostUrl.Host}";
            
            // Create the Url for the admin portal to get the App Only context for the Tenant
            var adminUrl = new Uri(baseUrl.Insert(baseUrl.IndexOf("."), "-admin"));

            // Get the access token
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                adminUrl.Authority,
                TokenHelper.GetRealmFromTargetUrl(adminUrl)).AccessToken;

            // Create Tenant Context
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(adminUrl.ToString(), accessToken))
            {
                Tenant tenant = new Tenant(ctx);

                // See if site collection exists
                var siteExists = tenant.SiteExists($"{baseUrl}/sites/{url}");

                // Works with the Remote validation attribute specified in NewSiteProperties
                // Return a string if there is an error, otherwise return true
                if (siteExists)
                {
                    return Json("There is already a site with that URL.  Please type a unique URL.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
        }

        [SharePointContextFilter]
        [HttpPost]
        public ActionResult CreateSite(NewSiteProperties props)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var baseUrl = $"{spContext.SPHostUrl.Scheme}://{spContext.SPHostUrl.Host}";

            // Create admin URL
            var adminUrl = new Uri(baseUrl.Insert(baseUrl.IndexOf("."), "-admin"));

            // Get the access token
            var accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                adminUrl.Authority,
                TokenHelper.GetRealmFromTargetUrl(adminUrl)).AccessToken;

            // Create the tenant ClientContext and create the site
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(adminUrl.ToString(), accessToken))
            {
                Tenant tenant = new Tenant(ctx);
                // Pass the false parameter for wait so we do not hold the connection open
                // while waiting for the site to be created.  Instead we show a spinner.
                tenant.CreateSiteCollection($"{baseUrl}/sites/{props.Url}", props.Title, props.SiteOwnerEmail, props.SelectedWebTemplate, 1000, 800, 7, 10, 8, 1033, false, false, null);
            }

            // Change the leaf URL to the AbsoluteUri so we can provide a link to the newly created site.
            props.SPHostUrl = spContext.SPHostUrl.AbsoluteUri;
            //return View();
            return RedirectToAction("SiteStatus", props);
        }

        [SharePointContextFilter]
        public ActionResult SiteStatus(NewSiteProperties props)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var baseUrl = $"{spContext.SPHostUrl.Scheme}://{spContext.SPHostUrl.Host}";
            
            // Build the admin URL
            var adminUrl = new Uri(baseUrl.Insert(baseUrl.IndexOf("."), "-admin"));

            // Get the access token
            string accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                adminUrl.Authority,
                TokenHelper.GetRealmFromTargetUrl(adminUrl)).AccessToken;

            // Check if the site exists and is "Active" if it does we return the View
            // If it is not "Active" yet, we return the "WaitingOnSite" view with a spinner
            using (var ctx = TokenHelper.GetClientContextWithAccessToken(adminUrl.ToString(), accessToken))
            {
                Tenant tenant = new Tenant(ctx);

                // Checks to see if site is created yet.
                var isSiteAvailable = tenant.CheckIfSiteExists($"{baseUrl}/sites/{props.Url}", "Active");


                if (!isSiteAvailable)
                {
                    // This view uses JavaScript to refresh every 10 seconds to check if the site has been created
                    return View("WaitingOnSite");
                }
                else
                {
                    // Convert the URL to Absolute so we can provide a link to the new site collection
                    props.Url = $"{baseUrl}/sites/{props.Url}";
                    return View(props);
                }
            }
        }

        [SharePointContextFilter]
        public ActionResult CreateWeb(NewWebProperties props)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            // Uses SPHost context and creates a subsite in the host web
            using (var cc = spContext.CreateUserClientContextForSPHost())
            {
                Web newWeb = cc.Web.CreateWeb(props.Title, props.Url, "", props.SelectedWebTemplate, 1033);

                cc.Load(newWeb);
                cc.ExecuteQueryRetry();

                // Convert the URL to absolute so we can provide a link to the new subsite
                props.Url = newWeb.Url;
                return View(props);
            }
        }

        [SharePointContextFilter]
        [AcceptVerbs("Get", "Post")]
        public ActionResult ValidateWeb(string url)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var ctx = spContext.CreateUserClientContextForSPHost())
            {
                // Works with the Remote validation attribute specified in NewWebProperties
                // Return a string if there is an error, otherwise return true
                if (ctx.Web.WebExists(url))
                {
                    return Json("Subsite with that URL already exists.", JsonRequestBehavior.AllowGet);
                }
                else
                {
                    return Json(true, JsonRequestBehavior.AllowGet);
                }
            }
        }
    }
}
