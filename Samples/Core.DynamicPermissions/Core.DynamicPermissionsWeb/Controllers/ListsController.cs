using Contoso.Core.DynamicPermissionsWeb.Models;
using Contoso.Core.DynamicPermissionsWeb.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Contoso.Core.DynamicPermissionsWeb.Controllers
{
    public class ListsController : Controller
    {
        //
        // GET: /Lists/
        public ActionResult Index()
        {
            TokenRepository repository = new TokenRepository(Request, Response);
            ListsViewModel model = new ListsViewModel();
            model.IsConnectedToO365 = repository.IsConnectedToO365;
            model.SiteTitle = repository.GetSiteTitle();            
            model.Lists = repository.GetLists();
            
            return View(model);
        }
        public ActionResult CreateList(string title)
        {
            TokenRepository repository = new TokenRepository(Request, Response);
            ListsViewModel model = new ListsViewModel();
            model.IsConnectedToO365 = repository.IsConnectedToO365;
            
            repository.CreateList(title);                
                        
            return RedirectToAction("Index");
            
        }
	}
}