using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace UnitTesting.SharePoint.AppsWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            ViewBag.UserName = GetCurrentUserTitle();

            return View();
        }

        public string GetCurrentUserTitle()
        {
            string spUserTitle = string.Empty;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    spUserTitle = spUser.Title;
                }
            }

            return spUserTitle;
        }

        public string GetAppOnlyCurrentUserTitle()
        {
            string spUserTitle = string.Empty;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateAppOnlyClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    spUserTitle = spUser.Title;
                }
            }

            return spUserTitle;
        }

        public string GetHostWebTitle()
        {
            string webTitle = string.Empty;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    var web = clientContext.Web;

                    clientContext.Load(web, w => w.Title);

                    clientContext.ExecuteQuery();

                    webTitle = web.Title;
                }
            }

            return webTitle;
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }
    }
}
