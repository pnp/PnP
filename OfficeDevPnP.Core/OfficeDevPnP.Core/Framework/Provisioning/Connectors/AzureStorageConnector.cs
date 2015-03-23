using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Connectors
{
    /// <summary>
    /// Connector for files in Azure storage
    /// </summary>
    public class AzureStorageConnector : FileConnectorBase
    {
        #region private variables
        private bool initialized = false;
        private CloudBlobClient blobClient = null;
        #endregion

        #region Constructor
        public AzureStorageConnector() : base()
        {

        }
        
        public AzureStorageConnector(string connectionString, string container): base ()
        {
            if (String.IsNullOrEmpty(connectionString))
            {
                throw new ArgumentException("connectionString");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }

            this.AddParameter(CONNECTIONSTRING, connectionString);
            this.AddParameter(CONTAINER, container);
        }
        #endregion

        #region overrides
        public override List<string> GetFiles()
        {
            return GetFiles(GetContainer());
        }

        public override List<string> GetFiles(string container)
        {
            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }

            if (!initialized)
            {
                Initialize();
            }

            List<string> result = new List<string>();

            CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);

            foreach (IListBlobItem item in blobContainer.ListBlobs(null, false))
            {
                if (item.GetType() == typeof(CloudBlockBlob))
                {
                    CloudBlockBlob blob = (CloudBlockBlob)item;
                    result.Add(blob.Name);
                }
            }

            return result;
        }

        public override string GetFile(string fileName)
        {
            return GetFile(fileName, GetContainer());
        }

        public override string GetFile(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }
 
            string result = null;
            MemoryStream stream = null;
            try
            {
                stream = GetFileFromStorage(fileName, container);

                if (stream == null)
                {
                    return null;
                }

                result = System.Text.Encoding.UTF8.GetString(stream.ToArray());
            }
            finally
            {
                if (stream != null)
                {
                    stream.Dispose();
                }
            }

            return result;
        }

        public override Stream GetFileStream(string fileName)
        {
            return GetFileStream(fileName, GetContainer());
        }

        public override Stream GetFileStream(string fileName, string container)
        {
            if (String.IsNullOrEmpty(fileName))
            {
                throw new ArgumentException("fileName");
            }

            if (String.IsNullOrEmpty(container))
            {
                throw new ArgumentException("container");
            }

            return GetFileFromStorage(fileName, container);
        }
        #endregion

        #region Private methods
        private void Initialize()
        {
            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(GetConnectionString());
            blobClient = storageAccount.CreateCloudBlobClient();
            initialized = true;
        }

        private MemoryStream GetFileFromStorage(string fileName, string container)
        {
            try
            {
                if (!initialized)
                {
                    Initialize();
                }

                CloudBlobContainer blobContainer = blobClient.GetContainerReference(container);
                CloudBlockBlob blockBlob = blobContainer.GetBlockBlobReference(fileName);

                MemoryStream result = new MemoryStream();
                blockBlob.DownloadToStream(result);
                return result;
            }
            catch (StorageException ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, "File {0} not found in Azure storage container {1}. Exception = {2}", fileName, container, ex.Message);
                return null;
            }
        }
        #endregion
    }
}
