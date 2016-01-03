using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            var result = MicrosoftGraphHelper.MakeGetRequestAsString("https://graph.microsoft.com/v1.0/me");
            return View();
        }

        public ActionResult MailCalendarContacts()
        {
            return View();
        }

        public ActionResult UsersGroups()
        {
            return View("Index");
        }

        public ActionResult Files()
        {
            return View("Index");
        }

        public ActionResult Others()
        {
            return View("Index");
        }
    }
}