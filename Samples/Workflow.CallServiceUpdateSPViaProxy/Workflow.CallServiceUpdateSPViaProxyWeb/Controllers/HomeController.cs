using System.Web.Mvc;
using Workflow.CallServiceUpdateSPViaProxyWeb.Services;

namespace Workflow.CallServiceUpdateSPViaProxyWeb.Controllers
{
    public class HomeController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Index()
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            ViewBag.AppWebUrl = spContext.SPAppWebUrl.ToString();
            return View();
        }
    }
}