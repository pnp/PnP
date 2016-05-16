using Office365Api.Graph.Simple.MailAndFiles.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365Api.Graph.Simple.MailAndFiles.Controllers
{
    public class PersonalDataController : Controller
    {
        // GET: PersonalData
        public ActionResult Index()
        {
            // Let's get the user details from the session, stored when user was signed in.
            if (Session[Helpers.SessionKeys.Login.UserInfo] != null)
            { 
                ViewBag.Name = (Session[Helpers.SessionKeys.Login.UserInfo] as UserInformation).Name;
            }
            return View();
        }
    }
}