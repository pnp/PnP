using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.WorkflowServices;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Web.Mvc;
using Workflow.CallServiceUpdateSPViaProxyWeb.Services;

namespace Workflow.CallServiceUpdateSPViaProxyWeb.Controllers
{
    public class PartSuppliersController : Controller
    {
        public ActionResult CreateGet(string spHostUrl)
        {
            ViewBag.Country = new SelectListItem[] {
                new SelectListItem { Text = "Australia", Value="Australia" },
                new SelectListItem { Text = "Canada", Value="Canada" },
                new SelectListItem { Text = "Finland", Value="Finland" },
                new SelectListItem { Text = "USA", Value="USA" }
            };

            if (ControllerContext.IsChildAction) return PartialView("Create");
            return PartialView("Create");
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult Create(string country, string spHostUrl)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var service = new PartSuppliersService(clientContext);
                var id = service.GetIdByCountry(country);
                if (id == null)
                {
                    id = service.Add(country);
                    TempData["Message"] = "Part Supplier Successfully Created!";
                }
                else
                    TempData["ErrorMessage"] = string.Format("Failed to Create The Part Supplier: There's already a Part Supplier who's country is {0}.", country);

                return RedirectToAction("Details", new { id = id.Value, SPHostUrl = spHostUrl });
            }
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult StartWorkflow(int id, Guid workflowSubscriptionId, string spHostUrl)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);

            var webServiceUrl = Url.RouteUrl("DefaultApi", new { httproute = "", controller = "Data" }, Request.Url.Scheme);
            var payload = new Dictionary<string, object>
                {
                    { "appWebUrl", spContext.SPAppWebUrl.ToString() },
                    { "webServiceUrl", webServiceUrl }
                };

            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var service = new PartSuppliersService(clientContext);
                service.StartWorkflow(workflowSubscriptionId, id, payload);
            }

            TempData["Message"] = "Workflow Successfully Started!";
            return RedirectToAction("Details", new { id = id, SPHostUrl = spHostUrl });
        }

        [HttpPost]
        [SharePointContextFilter]
        public ActionResult PublishSubmitEvent(int id, Guid workflowInstanceId, string spHostUrl)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var service = new PartSuppliersService(clientContext);
                service.PublishCustomEvent(workflowInstanceId, "Submit for Approval", "");
                
                // Wait until workflow user status changed
                WorkflowInstance instance;
                do
                {
                    Thread.Sleep(1000);
                    instance = service.GetWorkflowInstance(workflowInstanceId);
                }
                while (instance.UserStatus == "Wait for Submit");
            }
            TempData["Message"] = "Successfully Submitted For Approval!";


            return RedirectToAction("Details", new { id = id, SPHostUrl = spHostUrl });
        }

        [SharePointContextFilter]
        public ActionResult Details(int id)
        {
            var spContext = SharePointContextProvider.Current.GetSharePointContext(HttpContext);
            using (var clientContext = spContext.CreateUserClientContextForSPAppWeb())
            {
                var service = new PartSuppliersService(clientContext);
                var item = service.GetItem(id);
                if (item == null) return HttpNotFound();

                var workflowSubscription = service.GetWorkflowSubscription("Approve Suppliers");

                ViewBag.AppWebUrl = spContext.SPAppWebUrl.ToString();
                ViewBag.WorkflowSubscription = workflowSubscription;
                ViewBag.WorkflowInstance = service.GetItemWorkflowInstance(workflowSubscription.Id, id);
                ViewBag.ApprovalTask = service.GetApprovalTaskForCurrentUser(id);

                return View(item);
            }
        }
    }
}