using Microsoft.WindowsAzure.ServiceRuntime;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Queue;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Runtime.Serialization;
using System.ServiceModel;
using System.Text;
using System.Xml.Serialization;
using Contoso.Provisioning.Hybrid.Contract;


namespace Contoso.Provisioning.Hybrid.Web
{
    // NOTE: You can use the "Rename" command on the "Refactor" menu to change the class name "SharePointProvisioningService" in code, svc and config file together.
    // NOTE: In order to launch WCF Test Client for testing this service, please select SharePointProvisioningService.svc or SharePointProvisioningService.svc.cs at the Solution Explorer and start debugging.
    public class SharePointProvisioningService : ISharePointProvisioningService
    {
        //Queue name must be lowercase
        private const string queueName = "sharepointprovisioning";
        private const string azureConnectionSettingKey = "Microsoft.WindowsAzure.Plugins.Diagnostics.ConnectionString";
        private static CloudQueueClient queueStorage;
        private static bool storageInitialized = false;
        private static object gate = new Object();

        public SharePointProvisioningService()
        {
            InitializeStorage();
        }

        public bool ProvisionSiteCollection(SharePointProvisioningData sharePointProvisioningData)
        {
            bool success = false;

            try
            {
                // submit provisioning data to queue
                CloudQueue queue = queueStorage.GetQueueReference(queueName);
                CloudQueueMessage message = new CloudQueueMessage(SerializeData(sharePointProvisioningData));
                queue.AddMessage(message);
                success = true;
            }
            catch (Exception)
            {
                //Log error
                throw;
            }

            return success;
        }

        /// <summary>
        /// Serializes the SharePointProvisioningData object to XML to allow it to be passed to the worker role
        /// </summary>
        /// <param name="sharePointProvisioningData">SharePointProvisioningData object</param>
        /// <returns>XML representation as a string</returns>
        private static string SerializeData(SharePointProvisioningData sharePointProvisioningData)
        {
            string serializedSharePointProvisioningaData = "";
            using (Stream stream = new MemoryStream())
            {
                new XmlSerializer(typeof(SharePointProvisioningData)).Serialize(stream, sharePointProvisioningData);
                stream.Position = 0;
                serializedSharePointProvisioningaData = new StreamReader(stream).ReadToEnd();
            }
            return serializedSharePointProvisioningaData;
        }

        /// <summary>
        /// Initializes Azure storage and sets up the queue needed to communicate with the worker process
        /// </summary>
        private void InitializeStorage()
        {
            if (storageInitialized)
            {
                return;
            }

            lock (gate)
            {
                if (storageInitialized)
                {
                    return;
                }

                try
                {
                    string storageAccount = RoleEnvironment.GetConfigurationSettingValue(azureConnectionSettingKey);
                    // read account configuration settings
                    CloudStorageAccount cloudStorageAccount = CloudStorageAccount.Parse(storageAccount);
                    // Setup the queue to communicate with the worker role
                    queueStorage = cloudStorageAccount.CreateCloudQueueClient();
                    CloudQueue queue = queueStorage.GetQueueReference(queueName);
                    queue.CreateIfNotExists();
                }
                catch (WebException)
                {
                    throw new WebException("Storage services initialization failure. "
                        + "Check your storage account configuration settings. If running locally, "
                        + "ensure that the Development Storage service is running.");
                }

                storageInitialized = true;
            }
        }
    }
}
