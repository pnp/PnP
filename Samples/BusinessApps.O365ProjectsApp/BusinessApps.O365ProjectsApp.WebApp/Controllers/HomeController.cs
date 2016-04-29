using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.O365ProjectsApp.WebApp.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            ViewBag.Current = "Index";
            return View();
        }

        public ActionResult StartNewProcess()
        {
            ViewBag.Current = "StartNewProcess";
            return View("Index");
        }

        public ActionResult MyProcesses()
        {
            ViewBag.Current = "MyProcesses";
            return View("Index");
        }

        public ActionResult Settings()
        {
            ViewBag.Current = "Settings";
            return View("Index");
        }
    }
}