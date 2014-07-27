using Core.DataStorageModelsWeb.Services;
using Microsoft.SharePoint.Client;
using System.Web.Mvc;

namespace Core.DataStorageModelsWeb.Controllers
{
    public class CSRInfoController : Controller
    {
        [SharePointContextFilter]
        public ActionResult Home()
        {
            var context = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            var sharePointService = new SharePointService(context);
            var currentUser = sharePointService.GetCurrentUser();
            ViewBag.UserName = currentUser.Title;

            var surveyRatingsService = new SurveyRatingsService();
            ViewBag.Score = surveyRatingsService.GetUserScore(currentUser.UserId.NameId);

            return View();
        }
    }
}