using CorporateEvents.SharePointWeb.Models;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace CorporateEvents.SharePointWeb.Controllers {
    public class HomeController : Controller {

        [SharePointContextFilter]
        public ActionResult Index() {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            User spUser = null;

            using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                if (clientContext != null) {
                    spUser = clientContext.Web.CurrentUser;
                    clientContext.Load(clientContext.Web);
                    clientContext.Load(spUser, user => user.Title);
                    clientContext.ExecuteQuery();

                    var propertyValue = clientContext.Web.GetPropertyBagValueString("EventsConfigVersion", string.Empty);
                    if (propertyValue != ListDetails.CURRENT_EVENTS_CONFIGURATION_VERSION) {
                        return RedirectToAction("Config", new { SPHostUrl = spContext.SPHostUrl, SPLanguage = spContext.SPLanguage });
                    }

                    ViewBag.UserName = spUser.Title;
                }
            }

            return View();
        }

        public ActionResult Config(string status) {
            if (!string.IsNullOrEmpty(status))
                ViewBag.Status = status;

            return View();
        }

        [SharePointContextFilter]
        [HttpPost()]
        public ActionResult Config(System.Web.Mvc.FormCollection form) {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            string status;
            try {
                var initializer = new DataInitializer(spContext);
                initializer.Initialize();
                
                status = "Complete";
                using (var clientContext = HttpContext.GetUserClientContextForSPHost()) {
                    clientContext.Web.SetPropertyBagValue("EventsConfigVersion", "0.1.0.0");
                }
            }
            catch {
                status = "Error initializing the data store.";
            }
            return RedirectToAction("Config", new { status = status });
        }

        public ActionResult About() {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact() {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
