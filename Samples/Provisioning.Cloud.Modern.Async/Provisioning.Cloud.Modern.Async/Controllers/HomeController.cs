using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using Provisioning.Cloud.Modern.Async.Components;
using Provisioning.Cloud.Modern.Async.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Helpers;
using System.Web.Mvc;

namespace Provisioning.Cloud.Modern.Async.Controllers
{
    [Authorize]
    public class HomeController : Controller
    {
        public ActionResult Index()
        {
            // Just fire the OAuth access token request
            var token = MicrosoftGraphHelper.GetAccessTokenForCurrentUser();

            var model = new IndexViewModel();
            if (System.Security.Claims.ClaimsPrincipal.Current != null && System.Security.Claims.ClaimsPrincipal.Current.Identity != null && System.Security.Claims.ClaimsPrincipal.Current.Identity.IsAuthenticated)
            {
                model.CurrentUserPrincipalName = System.Security.Claims.ClaimsPrincipal.Current.Identity.Name;
            }

            return View(model);
        }

        [HttpPost]
        public ActionResult Create(IndexViewModel model)
        {
            AntiForgery.Validate();
            if (ModelState.IsValid)
            {
                // Set the current SPO Admin Site URL
                model.SPORootSiteUrl = ConfigurationManager.AppSettings["SPORootSiteUrl"];

                // Set the current user's access token
                model.UserAccessToken = MicrosoftGraphHelper.GetAccessTokenForCurrentUser(model.SPORootSiteUrl);

                // Get the JSON site creation information
                String modernSiteCreation = JsonConvert.SerializeObject(model);

                String targetQueue = String.Empty;

                // Determine the target asynchronous creation technique
                switch (model.AsyncTech)
                {
                    case AsynchronousTechnique.AzureFunction:
                        targetQueue = ConfigurationManager.AppSettings["AzureFunctionQueue"];
                        break;
                    case AsynchronousTechnique.AzureWebJob:
                    default:
                        targetQueue = ConfigurationManager.AppSettings["AzureWebJobQueue"];
                        break;
                }

                // Get the storage account for Azure Storage Queue
                CloudStorageAccount storageAccount =
                    CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["StorageQueue"].ConnectionString);

                // Get queue ... and create if it does not exist
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(targetQueue);
                queue.CreateIfNotExists();

                // Add entry to queue
                queue.AddMessage(new CloudQueueMessage(modernSiteCreation));
            }

            return View(model);
        }
    }
}