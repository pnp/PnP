using Core.CrossDomainLib.Models;
using Core.CrossDomainLibWeb;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Publishing;
using Microsoft.SharePoint.Client.Utilities;
using Microsoft.SharePoint.Client.WebParts;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.CrossDomainLib.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            ProvisionSample();

            ViewBag.SPHostUrl = this.Request.QueryString["SPHostUrl"];
            ViewBag.SPHost = "https://" + new Uri(this.Request.QueryString["SPHostUrl"]).Host;

            return View();
        }

        private void ProvisionSample()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                var provisionner = new Provisionner(clientContext, this.Server, this.Request);

                ViewBag.GetSampleUrl = provisionner.ProvisionGetSample();
                ViewBag.PostSampleUrl = provisionner.ProvisionPostSample();
                ViewBag.ViewWithPostSampleUrl = provisionner.ProvisionViewWithPostSample();
            }
        }

        [SharePointContextFilter]
        public ActionResult Proxy()
        {
            return View(); //this page is used as a proxy by the cross domain lib. The view should be a empty html page with references to jquerry and CrossDomainProxy.js
        }

        public ActionResult TestView()
        {
            //This action is called by the "view with post" sample. On load of the library, this code is called to get html and javascript to render the gray form on the SharePoint page.
            ViewBag.ServerUrl = "https://" + this.Request.Url.Authority;
            return View();
        }

        [HttpGet]
        public ActionResult TestGet(string id)
        {
            //This action is called by the "get sample"
            var userName = GetCurrentUsername();  //a call to SharePoint is made to get username
            var user = string.Format("Id passed to function is {0}. Current user fetched by C# CSOM on server: {1}.", id, userName);
            return Json(user, JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        public ActionResult TestPost(string name, string street)
        {
            //This action is called by the "post sample"
            //in this action you can call sharepoint using the standard way
            return Json(string.Format("Provided data: {0} - {1}, was recieved on the server", name, street), JsonRequestBehavior.DenyGet);
        }

        private string GetCurrentUsername()
        {
            User spUser = null;

            //sharepoint and clientcontext are created in normal way... Authentication is handled by cross domain lib ([SharePointContextFilter] needs to be present on controller!!!)
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            using (var clientContext = spContext.CreateUserClientContextForSPHost())
            {
                if (clientContext != null)
                {
                    spUser = clientContext.Web.CurrentUser;

                    clientContext.Load(spUser, user => user.Title);

                    clientContext.ExecuteQuery();

                    return spUser.Title;
                }
            }

            return string.Empty;
        }
    }
}
