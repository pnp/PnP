using MVCTaxonomyPickerWeb.Helpers;
using MVCTaxonomyPickerWeb.Models;
using MVCTaxonomyPickerWeb.Services;
using MVCTaxonomyPickerWeb.ViewModels;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MVCTaxonomyPickerWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            User spUser = null;

            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

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

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult GetTaxonomyPickerData(TermSetQueryModel model)
        {
            return Json(TaxonomyPickerService.GetTaxonomyPickerData(model), JsonRequestBehavior.AllowGet);
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult AddTaxonomyTerm(TermQueryModel model)
        {
            return Json(TaxonomyPickerService.AddTerm(model), JsonRequestBehavior.AllowGet);
        }

        [HttpGet]
        [SharePointContextFilter]
        public ActionResult DeleteTaxonomyTerm(TermQueryModel model)
        {
            return Json(TaxonomyPickerService.DeleteTerm(model), JsonRequestBehavior.AllowGet);
        }
        
        [SharePointContextFilter]
        public ActionResult TaxonomyPickerDemo()
        {
            var model = new DemoViewModel();            
            return View(model);
        }

        [HttpPost]        
        [SharePointContextFilter]
        public ActionResult GetTaxonomyPickerHiddenValue(DemoViewModel model)
        {
            if (!ModelState.IsValid) //Check for validation errors
            {
                RedirectToAction("Index", "Home");
            }
            return Json(JsonHelper.Serialize<DemoViewModel>(model), JsonRequestBehavior.AllowGet);
        }
    }
}
