using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class UsersGroupsController : Controller
    {
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlayWithUsers()
        {
            var users = UsersHelper.ListUsers(600);
            var paolo = UsersHelper.GetUser("paolo@piasysdev.onmicrosoft.com");
            var paoloMFA = UsersHelper.GetUser("paoloMFA@piasysdev.onmicrosoft.com");
            var paoloADFS = UsersHelper.GetUser("paolo.pialorsi@sharepoint-camp.com");

            var groups = UsersHelper.ListGroups(100);

            return View("Index");
        }

        public ActionResult PlayWithSecurityGroups()
        {
            return View("Index");
        }

        public ActionResult PlayWithUnifiedGroups()
        {
            return View("Index");
        }
    }
}