using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization.Controllers
{
    public class GlobalAdminController : Controller
    {
        // GET: GlobalAdmin
        [AuthorizeUser(Roles = "admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}