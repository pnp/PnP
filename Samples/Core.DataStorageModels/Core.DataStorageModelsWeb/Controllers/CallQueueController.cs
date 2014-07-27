using Core.DataStorageModelsWeb.Services;
using System;
using System.Web.Mvc;

namespace Core.DataStorageModelsWeb.Controllers
{
    [SharePointContextFilter]
    public class CallQueueController : Controller
    {
        public CallQueueService CallQueueService { get; private set; }

        public CallQueueController()
        {
            CallQueueService = new CallQueueService();
        }

        // GET: CallQueue
        public ActionResult Home(UInt16 displayCount = 10)
        {
            var calls = CallQueueService.PeekCalls(displayCount);
            ViewBag.DisplayCount = displayCount;
            ViewBag.TotalCallCount = CallQueueService.GetCallCount();
            return View(calls);
        }

        [HttpPost]
        public ActionResult SimulateCalls(string spHostUrl)
        {
            int count = CallQueueService.SimulateCalls();
            TempData["Message"] = string.Format("Successfully simulated {0} calls and added them to the call queue.", count);
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }

        [HttpPost]
        public ActionResult TakeCall(string spHostUrl)
        {
            CallQueueService.DequeueCall();
            TempData["Message"] = "Call taken successfully and removed from the call queue!";
            return RedirectToAction("Home", new { SPHostUrl = spHostUrl });
        }
    }
}