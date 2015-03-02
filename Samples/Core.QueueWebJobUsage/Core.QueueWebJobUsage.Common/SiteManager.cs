using Microsoft.SharePoint.Client;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Core.QueueWebJobUsage.Common
{
    /// <summary>
    /// Imaginary business logic handler which contains the needed logic to be applied to the host web.
    /// </summary>
    public class SiteManager
    {

        #region CONSTANTS

        public const string StorageQueueName = "asynchostweboperation";

        #endregion

        /// <summary>
        /// Used to add new message to storage queue for processing
        /// </summary>
        /// <param name="modifyRequest">Request object with needed details</param>
        /// <param name="storageConnectionString">Storage connection string</param>
        public void AddAsyncOperationRequestToQueue(SiteModifyRequest modifyRequest, 
                                                    string storageConnectionString)
        {
            CloudStorageAccount storageAccount =
                                CloudStorageAccount.Parse(storageConnectionString);

            // Get queue... create if does not exist.
            CloudQueueClient queueClient = storageAccount.CreateCloudQueueClient();
            CloudQueue queue = queueClient.GetQueueReference(SiteManager.StorageQueueName);
            queue.CreateIfNotExists();

            // Add entry to queue
            queue.AddMessage(new CloudQueueMessage(JsonConvert.SerializeObject(modifyRequest)));
        }


        public void PerformSiteModification(ClientContext ctx, SiteModifyRequest modifyRequest)
        {
            // Sleep 10 sec to show the challenge
            Thread.Sleep(10000);

            // Perform simple operation by adding new document lib to host web
            CreateDocLibrary(ctx, Guid.NewGuid().ToString().Replace("-", ""), string.Format("Requested by {0}", modifyRequest.RequestorName));
        }

        public void CreateDocLibrary(ClientContext ctx, string libraryName, string description)
        {

            // Create new list to the host web
            ListCreationInformation list = new ListCreationInformation();
            list.Title = libraryName;
            list.TemplateType = (int)ListTemplateType.DocumentLibrary;
            list.Description = description;
            list.Url = libraryName;
            ctx.Web.Lists.Add(list);
            ctx.ExecuteQuery();
        }
    }
}
