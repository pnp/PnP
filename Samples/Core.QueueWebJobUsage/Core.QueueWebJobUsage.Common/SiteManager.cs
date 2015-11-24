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
            // Sleep 20 sec to show the challenge
            Thread.Sleep(20000);

            // Perform simple operation by adding new document lib to host web
            CreateDocLibrary(ctx, DateTime.Now.Ticks.ToString(), string.Format("Requested by {0}", modifyRequest.RequestorName));
        }

        public void CreateDocLibrary(ClientContext ctx, string libraryName, string requestor)
        {

            // Create new list to the host web
            ListCreationInformation newList = new ListCreationInformation();
            newList.Title = libraryName;
            newList.TemplateType = (int)ListTemplateType.GenericList;
            newList.Description = requestor;
            newList.Url = libraryName;
            List list = ctx.Web.Lists.Add(newList);

            ListItemCreationInformation newItem = new ListItemCreationInformation();
            ListItem item = list.AddItem(newItem);
            item["Title"] = requestor;
            item.Update();
            ctx.ExecuteQuery();
        }
    }
}
