using System.Web.Mvc;
using Workflow.CallCustomServiceWeb.Services;

namespace Workflow.CallCustomServiceWeb.Controllers
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