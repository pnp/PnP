using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization.Controllers
{
    public class CompanyAdminController : Controller
    {
        [AuthorizeUser(Roles = "Company Admin")]
        // GET: CompanyAdmin
        public ActionResult Index()
        {
            return View();
        }
    }
}