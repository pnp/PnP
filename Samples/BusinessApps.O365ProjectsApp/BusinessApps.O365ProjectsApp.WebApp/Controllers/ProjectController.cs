using BusinessApps.O365ProjectsApp.Infrastructure;
using BusinessApps.O365ProjectsApp.Infrastructure.MicrosoftGraph;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace BusinessApps.O365ProjectsApp.WebApp.Controllers
{
    public class ProjectController : Controller
    {
        // GET: Project
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult StartNewProject()
        {
            Guid groupId = Guid.NewGuid();
            var groupNameId = groupId.ToString().Replace("-", "");

            MemoryStream memPhoto = new MemoryStream();
            using (FileStream fs = new FileStream(Server.MapPath("~/AppIcon.png"), 
                FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Byte[] newPhoto = new Byte[fs.Length];
                fs.Read(newPhoto, 0, (Int32)(fs.Length - 1));
                memPhoto.Write(newPhoto, 0, newPhoto.Length);
                memPhoto.Position = 0;
            }

            GroupCreationInformation job = new GroupCreationInformation {
                AccessToken = MicrosoftGraphHelper.GetAccessTokenForCurrentUser(
                    O365ProjectsAppSettings.MicrosoftGraphResourceId),
                JobId = groupId,
                Name = groupNameId,
                Members = new String[] {
                        "paolo@piasysdev.onmicrosoft.com",
                        "paolo.pialorsi@sharepoint-camp.com"
                    },
                Photo = memPhoto.ToArray(),
            };

            try
            {
                // Get the storage account for Azure Storage Queue
                CloudStorageAccount storageAccount =
                    CloudStorageAccount.Parse(ConfigurationManager.ConnectionStrings["AzureWebJobsStorage"].ConnectionString);

                // Get queue ... and create if it does not exist
                CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
                CloudQueue queue = queueClient.GetQueueReference(O365ProjectsAppConstants.Blob_Storage_Queue_Name);
                queue.CreateIfNotExists();

                // Add entry to queue
                queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(job)));
            }
            catch (Exception)
            {
                // TODO: Handle any exception thrown by the object of type CloudQueue
            }

            return View();
        }
    }
}