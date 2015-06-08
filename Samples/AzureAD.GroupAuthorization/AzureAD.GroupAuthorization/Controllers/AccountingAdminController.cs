using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Office365AddIn.GroupAuthorization.Controllers
{
    public class AccountingAdminController : Controller
    {
        // GET: AccountingAdmin
        [AuthorizeUser(Roles = "Accounting Module Admin")]
        public ActionResult Index()
        {
            return View();
        }
    }
}