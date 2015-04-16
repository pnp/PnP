using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Core.DataStorageModelsWeb.Controllers
{
    public class SupportCaseAppPartController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index(string customerID)
        {
            ViewBag.SharePointContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            return View((object)customerID);
        }
    }
}
