using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization.Controllers
{
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            if (User.IsInRole("admin"))
            {
                return RedirectToAction("Index", "GlobalAdmin");
            }
            if (User.IsInRole("Company Admin"))
            {
                return RedirectToAction("Index", "CompanyAdmin");
            }
            if (User.IsInRole("Accounting Module Admin"))
            {
                return RedirectToAction("Index", "AccountingAdmin");
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
    }
}