using Core.DataStorageModelsWeb.Services;
using System.Web.Mvc;

namespace Core.DataStorageModelsWeb.Controllers
{
    [SharePointContextFilter]
    public class DefaultController : Controller
    {
        public ActionResult Home(string spHostUrl, string SPAppWebUrl)
        {
            return View();
        }

        [HttpPost]
        public ActionResult Deploy(string spHostUrl)
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var service = new SharePointService(context);
            TempData["Message"] = service.Deploy();
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }

        //Specifies the maximum number of list or library items that a database operation, 
        //such as a query, can process at the same time outside the daily time window set by the administrator during which queries are unrestricted.
        //http://technet.microsoft.com/en-us/library/cc262787
        [HttpPost]
        public ActionResult FillAppWebNotesListToThreshold(string spHostUrl)
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var service = new SharePointService(context);
            TempData["Message"] = service.FillAppWebNotesListToThreshold();
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }

        //Specifies the maximum number of list or library items that a database operation, 
        //such as a query, can process at the same time outside the daily time window set by the administrator during which queries are unrestricted.
        //http://technet.microsoft.com/en-us/library/cc262787
        [HttpPost]
        public ActionResult FillHostWebSupportCasesToThreshold(string spHostUrl)
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var service = new SharePointService(context);
            TempData["Message"] = service.FillHostWebSupportCasesToThreshold();
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }

        [HttpPost]
        public ActionResult UninstallTheApp(string spHostUrl)
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var service = new SharePointService(context);
            service.UninstallTheApp();
            TempData["Message"] = "The App has been uninstalled successfully.";
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
     
        }

        [HttpPost]
        public ActionResult FillAppWebNotesWith1G(string spHostUrl)
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var service = new SharePointService(context);
            var count = service.FillAppWebNotesWith1G();
            TempData["Message"] = count + " items have been added to the App Web Notes list, and every item size is more than 1MB.";
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }
    }
}