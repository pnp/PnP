using Contoso.Core.DynamicPermissionsWeb.Services;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contoso.Core.DynamicPermissionsWeb.Controllers
{
    public class HomeController : Controller
    {        
        public ActionResult Index()
        {
            TokenRepository repository = new TokenRepository(Request,Response);
            Models.IndexViewModel model = new Models.IndexViewModel
            {
                IsConnectedToO365 = repository.IsConnectedToO365,
                SiteTitle = repository.GetSiteTitle()
            };
                                            
            return View(model);
        }


        public ActionResult Connect(string hostUrl)
        {
            TokenRepository repository = new TokenRepository(Request, Response);
            repository.Connect(hostUrl);
            return View();            
        }

        public ActionResult Callback(string code)
        {
            TokenRepository repository = new TokenRepository(Request, Response);
            repository.Callback(code);
            return RedirectToAction("Index");
        }
    }
}
