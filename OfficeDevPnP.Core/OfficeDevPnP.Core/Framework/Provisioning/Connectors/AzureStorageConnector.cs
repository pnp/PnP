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
    /// Connector for files in Azure blob storage
    /// </summary>
    public class AzureStorageConnector : FileConnectorBase
    {
        #region private variables
        private bool initialized = false;
        private CloudBlobClient blobClient = null;
        #endregion

        #region Constructor
        /// <summary>
        /// Base constructor
        /// </summary>
        public AzureStorageConnector() : base()
        {

        }
        
        /// <summary>
        /// AzureStorageConnector constructor. Allows to directly set Azure Storage key and container
        /// </summary>
        /// <param name="connectionString">Azure Storage Key (DefaultEndpointsProtocol=https;AccountName=yyyy;AccountKey=xxxx)</param>
        /// <param name="container">Name of the Azure container to operate against</param>
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

            this.AddParameterAsString(CONNECTIONSTRING, connectionString);
            this.AddParameterAsString(CONTAINER, container);
        }
        #endregion

        #region Base class overrides
        /// <summary>
        /// Get the files available in the default container
        /// </summary>
        /// <returns>List of files</returns>
        public override List<string> GetFiles()
        {
            return GetFiles(GetContainer());
        }

        /// <summary>
        /// Get the files available in the specified container
        /// </summary>
        /// <param name="container">Name of the container to get the files from</param>
        /// <returns>List of files</returns>
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

        /// <summary>
        /// Gets a file as string from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public override string GetFile(string fileName)
        {
            return GetFile(fileName, GetContainer());
        }

        /// <summary>
        /// Gets a file as string from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
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

        /// <summary>
        /// Gets a file as stream from the default container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <returns>String containing the file contents</returns>
        public override Stream GetFileStream(string fileName)
        {
            return GetFileStream(fileName, GetContainer());
        }

        /// <summary>
        /// Gets a file as stream from the specified container
        /// </summary>
        /// <param name="fileName">Name of the file to get</param>
        /// <param name="container">Name of the container to get the file from</param>
        /// <returns>String containing the file contents</returns>
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
            try
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(GetConnectionString());
                blobClient = storageAccount.CreateCloudBlobClient();
                initialized = true;
            }
            catch(Exception ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_Azure_FailedToInitialize, ex.Message);
                throw;
            }
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

                Log.Info(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_Azure_FileRetrieved, fileName, container);
                return result;
            }
            catch (StorageException ex)
            {
                Log.Error(Constants.LOGGING_SOURCE, CoreResources.Provisioning_Connectors_Azure_FileNotFound, fileName, container, ex.Message);
                return null;
            }
        }
        #endregion
    }
}
