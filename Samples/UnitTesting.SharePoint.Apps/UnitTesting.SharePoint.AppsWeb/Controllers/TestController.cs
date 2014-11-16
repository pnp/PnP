using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Configuration;
using System.Web.Mvc;

namespace UnitTesting.SharePoint.AppsWeb.Controllers
{
    public class TestController : Controller
    {

        [SharePointContextFilter]
        public ActionResult Index()
        {
            var spContext = HttpContext.Session["SPContext"] as SharePointAcsContext;

            ViewBag.ContextToken = spContext.ContextToken;

            ViewBag.ClientId = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientId")) ? WebConfigurationManager.AppSettings.Get("HostedAppName") : WebConfigurationManager.AppSettings.Get("ClientId");

            ViewBag.ClientSecret = string.IsNullOrEmpty(WebConfigurationManager.AppSettings.Get("ClientSecret")) ? WebConfigurationManager.AppSettings.Get("HostedAppSigningKey") : WebConfigurationManager.AppSettings.Get("ClientSecret");

            return View();

        }
    }


}