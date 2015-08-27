using Microsoft.SharePoint.Client;
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
    public class ConnectorAzureTests
    {
        #region Test variables
        static string testContainer = "pnptest";
        static string testContainerSecure = "pnptestsecure";
        #endregion

        #region Test initialize and cleanup
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            // Azure setup
            if (!String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
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
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            // Azure setup
            if (!String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                CloudStorageAccount storageAccount = CloudStorageAccount.Parse(TestCommon.AzureStorageKey);
                CloudBlobClient blobClient = storageAccount.CreateCloudBlobClient();

                CloudBlobContainer container = blobClient.GetContainerReference(testContainer);
                container.DeleteIfExists();

                CloudBlobContainer containerSecure = blobClient.GetContainerReference(testContainerSecure);
                containerSecure.DeleteIfExists();
            }
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

        /// <summary>
        /// Save file to default container
        /// </summary>
        [TestMethod]
        public void AzureConnectorSaveStream1Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                byteCount = fileStream.Length;
                azureConnector.SaveFileStream("blabla.png", fileStream);
            }

            //read the file
            using (var bytes = azureConnector.GetFileStream("blabla.png"))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test since the used storage containers are deleted
        }

        /// <summary>
        /// Save file to specified container
        /// </summary>
        [TestMethod]
        public void AzureConnectorSaveStream2Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                byteCount = fileStream.Length;
                azureConnector.SaveFileStream("blabla.png", testContainer, fileStream);
            }

            //read the file
            using (var bytes = azureConnector.GetFileStream("blabla.png", testContainer))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test since the used storage containers are deleted
        }

        /// <summary>
        /// Save file to specified container, ensure the overwrite works
        /// </summary>
        [TestMethod]
        public void AzureConnectorSaveStream3Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);
            // first save
            using (var fileStream = System.IO.File.OpenRead(@".\resources\custombg.jpg"))
            {
                azureConnector.SaveFileStream("blabla.png", testContainer, fileStream);
            }

            // Second save
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                byteCount = fileStream.Length;
                azureConnector.SaveFileStream("blabla.png", testContainer, fileStream);
            }

            //read the file
            using (var bytes = azureConnector.GetFileStream("blabla.png", testContainer))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test since the used storage containers are deleted
        }

        /// <summary>
        /// Delete file from default container
        /// </summary>
        [TestMethod]
        public void AzureConnectorDelete1Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);

            // Add a file
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                azureConnector.SaveFileStream("blabla.png", fileStream);
            }

            // Delete the file
            azureConnector.DeleteFile("blabla.png");

            //read the file
            using (var bytes = azureConnector.GetFileStream("blabla.png"))
            {
                Assert.IsNull(bytes);
            }

            // file will be deleted at end of test since the used storage containers are deleted
        }

        /// <summary>
        /// Delete file from a specific container
        /// </summary>
        [TestMethod]
        public void AzureConnectorDelete2Test()
        {
            if (String.IsNullOrEmpty(TestCommon.AzureStorageKey))
            {
                Assert.Inconclusive("No Azure Storage Key defined in App.Config, so can't test");
            }

            AzureStorageConnector azureConnector = new AzureStorageConnector(TestCommon.AzureStorageKey, testContainerSecure);

            // Add a file
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                azureConnector.SaveFileStream("blabla.png", testContainer, fileStream);
            }

            // Delete the file
            azureConnector.DeleteFile("blabla.png", testContainer);

            //read the file
            using (var bytes = azureConnector.GetFileStream("blabla.png", testContainer))
            {
                Assert.IsNull(bytes);
            }

            // file will be deleted at end of test since the used storage containers are deleted
        }
        #endregion
    }
}
