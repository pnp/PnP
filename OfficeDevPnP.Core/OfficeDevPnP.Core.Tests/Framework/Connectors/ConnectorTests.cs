using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.WindowsAzure.Storage;
using Microsoft.WindowsAzure.Storage.Blob;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Tests.Framework.Connectors
{
    [TestClass]
    public class ConnectorTests
    {
        #region Test variables
        static string testContainer = "pnptest";
        static string testContainerSecure = "pnptestsecure";
        #endregion

        #region Test initialize and cleanup
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                return;
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(TestCommon.AzureStorageKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();
            
            CloudBlobContainer container = blobClient.GetContainerReference(testContainer);
            // Create the container if it doesn't already exist.
            container.CreateIfNotExists();

            // Upload files
            CloudBlockBlob blockBlob = container.GetBlockBlobReference("office365.png");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                blockBlob.UploadFromStream(fileStream);
            } 

            CloudBlobContainer containerSecure = blobClient.GetContainerReference(testContainerSecure);
            // Create the container if it doesn't already exist.
            containerSecure.CreateIfNotExists();

            // Avoid public access to this test container
            BlobContainerPermissions bcp = new BlobContainerPermissions();
            bcp.PublicAccess = BlobContainerPublicAccessType.Off;
            containerSecure.SetPermissions(bcp);

            blockBlob = containerSecure.GetBlockBlobReference("custom.spcolor");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@".\resources\custom.spcolor"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            blockBlob = containerSecure.GetBlockBlobReference("custombg.jpg");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@".\resources\custombg.jpg"))
            {
                blockBlob.UploadFromStream(fileStream);
            }

            blockBlob = containerSecure.GetBlockBlobReference("ProvisioningTemplate-2015-03-Sample-01.xml");
            // Create or overwrite the "myblob" blob with contents from a local file.
            using (var fileStream = System.IO.File.OpenRead(@".\resources\templates\ProvisioningTemplate-2015-03-Sample-01.xml"))
            {
                blockBlob.UploadFromStream(fileStream);
            }
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                return;
            }

            CloudStorageAccount storageAccount = CloudStorageAccount.Parse(TestCommon.AzureStorageKey);
            CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

            CloudBlobContainer container = blobClient.GetContainerReference(testContainer);
            container.DeleteIfExists();

            CloudBlobContainer containerSecure = blobClient.GetContainerReference(testContainerSecure);
            containerSecure.DeleteIfExists();
        }
        #endregion

        #region Azure connector tests

        /// <summary>
        /// Pass the connection information as parameters
        /// Get a file as string from passed Azure storage account and container
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFile1Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector();
            azureConnector.Parameters.Add(AzureStorageConnector.CONNECTIONSTRING, TestCommon.AzureStorageKey);
            azureConnector.Parameters.Add(AzureStorageConnector.CONTAINER, testContainerSecure);

            string file = azureConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml");
            Assert.IsNotNull(file);

            string file2 = azureConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Pass the connection information as constructor parameters
        /// Get a file as string from passed Azure storage account and container 
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFile2Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);

            string file = azureConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml");
            Assert.IsNotNull(file);

            string file2 = azureConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        /// <summary>
        /// List the files in the specified Azure storage account and container
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFiles1Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);
            var files = azureConnector.GetFiles();
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// List the files in the specified Azure storage account and container. Override container by specifying it in the GetFiles method
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFiles2Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);
            var files = azureConnector.GetFiles(testContainer);
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// Get file as stream from the specified Azure storage account and container
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFileBytes1Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);

            using (var bytes = azureConnector.GetFileStream("ProvisioningTemplate-2015-03-Sample-01.xml"))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = azureConnector.GetFileStream("Idonotexist.xml"))
            {
                Assert.IsNull(bytes2);
            }
        }

        /// <summary>
        /// Get file as stream from the specified Azure storage account and container. Override container by specifying it in the GetFileStream method
        /// </summary>
        [TestMethod]
        public void AzureConnectorGetFileBytes2Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);

            using (var bytes = azureConnector.GetFileStream("office365.png", testContainer))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = azureConnector.GetFileStream("Idonotexist.xml", testContainer))
            {
                Assert.IsNull(bytes2);
            }
        }
        #endregion

        #region File connector tests
        /// <summary>
        /// Get file as string from provided directory and folder. Specify both directory and container
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFile1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "Templates");

            string file = fileSystemConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml");
            Assert.IsNotNull(file);

            string file2 = fileSystemConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }
        
        /// <summary>
        /// Get file as string from provided directory and folder. Specify both directory and container, but container contains multiple elements
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFile2Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".", @"Resources\Templates");

            string file = fileSystemConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml");
            Assert.IsNotNull(file);

            string file2 = fileSystemConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Get file as string from provided directory and folder. Specify only directory and container, but override the container in the GetFile method
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFile3Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".", @"wrong");

            string file = fileSystemConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml", @"Resources\Templates");
            Assert.IsNotNull(file);

            string file2 = fileSystemConnector.GetFile("Idonotexist.xml", "Templates");
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Get files in the specified directory
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFiles1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "Templates");
            var files = fileSystemConnector.GetFiles();
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// Get files in the specified directory, override the set container in the GetFiles method
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFiles2Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "");
            var files = fileSystemConnector.GetFiles("Templates");
            Assert.IsTrue(files.Count > 0);

            var files2 = fileSystemConnector.GetFiles("");
            Assert.IsTrue(files2.Count > 0);
        }

        /// <summary>
        /// Get file as stream.
        /// </summary>
        [TestMethod]
        public void FileConnectorGetFileBytes1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "");

            using (var bytes = fileSystemConnector.GetFileStream("office365.png"))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = fileSystemConnector.GetFileStream("Idonotexist.xml"))
            {
                Assert.IsNull(bytes2);
            }
        }
        #endregion
    }
}
