using Microsoft.SharePoint.Client;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.DisplayCalendarEventsWeb.Controllers {
    public class HomeController : Controller {
        [SharePointContextFilter]
        public ActionResult Index() {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            // Save sharepoint context token into current session
            System.Web.HttpContext.Current.Session["SPContext"] = spContext;
            var spHostUrl = System.Web.HttpContext.Current.Request.QueryString["SPHostUrl"];

            var response = new JObject();
            response["spHostWebUrl"] = spHostUrl;
            //response["spAppWebUrl"] = spContext.SPAppWebUrl.AbsoluteUri;

            ViewBag.SpUrls = response.ToString();

            return View();
        }
    }
}
