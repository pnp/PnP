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
    public class ConnectorSharePointTests
    {
        #region Test variables
        static string testContainer = "pnptest";
        static string testContainerSecure = "pnptestsecure";
        #endregion

        #region Test initialize and cleanup
        [ClassInitialize()]
        public static void ClassInit(TestContext context)
        {
            // SharePoint setup
            using (ClientContext cc = TestCommon.CreateClientContext())
            {
                if (!cc.Web.ListExists(testContainer))
                {
                    List list = cc.Web.CreateDocumentLibrary(testContainer);
                    // upload files
                    list.RootFolder.UploadFile("office365.png", @".\resources\office365.png", true);
                }

                if (!cc.Web.ListExists(testContainerSecure))
                {
                    List list = cc.Web.CreateDocumentLibrary(testContainerSecure);
                    // upload files
                    list.RootFolder.UploadFile("custom.spcolor", @".\resources\custom.spcolor", true);
                    list.RootFolder.UploadFile("custombg.jpg", @".\resources\custombg.jpg", true);
                    list.RootFolder.UploadFile("ProvisioningTemplate-2015-03-Sample-01.xml", @".\resources\templates\ProvisioningTemplate-2015-03-Sample-01.xml", true);

                    // add files to folder structure
                    Folder sub1 = list.RootFolder.CreateFolder("sub1");
                    sub1.UploadFile("custom.spcolor", @".\resources\custom.spcolor", true);
                    sub1.UploadFile("custombg.jpg", @".\resources\custombg.jpg", true);

                    Folder sub11 = sub1.CreateFolder("sub11");
                    sub11.UploadFile("ProvisioningTemplate-2015-03-Sample-01.xml", @".\resources\templates\ProvisioningTemplate-2015-03-Sample-01.xml", true);
                }
            }
        }

        [ClassCleanup()]
        public static void ClassCleanup()
        {
            // SharePoint setup
            using (ClientContext cc = TestCommon.CreateClientContext())
            {
                if (cc.Web.ListExists(testContainer))
                {
                    List list = cc.Web.GetListByTitle(testContainer);
                    list.DeleteObject();
                    cc.ExecuteQueryRetry();

                    list = cc.Web.GetListByTitle(testContainerSecure);
                    list.DeleteObject();
                    cc.ExecuteQueryRetry();
                }
            }
        }
        #endregion

        #region SharePoint Connector tests
        /// <summary>
        /// Pass the connection information as parameters
        /// Get a file as string from passed SharePoint url and list
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFile1Test()
        {
            SharePointConnector spConnector = new SharePointConnector();
            spConnector.Parameters.Add(AzureStorageConnector.CONNECTIONSTRING, TestCommon.DevSiteUrl);
            spConnector.Parameters.Add(AzureStorageConnector.CONTAINER, testContainerSecure);
            spConnector.Parameters.Add(SharePointConnector.CLIENTCONTEXT, TestCommon.CreateClientContext());

            string file = spConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml");
            Assert.IsNotNull(file);

            string file2 = spConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Pass the connection information as parameters
        /// Get a file as string from passed SharePoint url and list. Uses 2 levels of sub folders 
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFile2Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainerSecure);

            string file = spConnector.GetFile("ProvisioningTemplate-2015-03-Sample-01.xml", String.Format("{0}/sub1/sub11", testContainerSecure));
            Assert.IsNotNull(file);

            string file2 = spConnector.GetFile("Idonotexist.xml", String.Format("{0}/sub1/sub11", testContainerSecure));
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Get files in the specified site and library
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFiles1Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainerSecure);
            var files = spConnector.GetFiles();
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// Get files in the specified site and library, override the set library in the GetFiles method
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFiles2Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainerSecure);
            var files = spConnector.GetFiles(String.Format("{0}/sub1", testContainerSecure));
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// Get files in the specified site and library, override the set library in the GetFiles method
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFiles3Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainerSecure);
            var files = spConnector.GetFiles(String.Format("{0}/sub1/sub11", testContainerSecure));
            Assert.IsTrue(files.Count > 0);
        }

        /// <summary>
        /// Get file as stream.
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFileBytes1Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);

            using (var bytes = spConnector.GetFileStream("office365.png"))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = spConnector.GetFileStream("Idonotexist.xml"))
            {
                Assert.IsNull(bytes2);
            }
        }

        /// <summary>
        /// Pass the connection information as parameters
        /// Get a file as stream from passed SharePoint url and list. Uses 1 level of sub folders 
        /// </summary>
        [TestMethod]
        public void SharePointConnectorGetFileBytes2Test()
        {
            SharePointConnector spConnector = new SharePointConnector();
            spConnector.Parameters.Add(AzureStorageConnector.CONNECTIONSTRING, TestCommon.DevSiteUrl);
            spConnector.Parameters.Add(AzureStorageConnector.CONTAINER, testContainerSecure);
            spConnector.Parameters.Add(SharePointConnector.CLIENTCONTEXT, TestCommon.CreateClientContext());

            using (var bytes = spConnector.GetFileStream("custombg.jpg", String.Format("{0}/sub1", testContainerSecure)))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            string file2 = spConnector.GetFile("Idonotexist.xml", String.Format("{0}/sub1", testContainerSecure));
            Assert.IsNull(file2);
        }

        /// <summary>
        /// Save file to default container
        /// </summary>
        [TestMethod]
        public void SharePointConnectorSaveStream1Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                byteCount = fileStream.Length;
                spConnector.SaveFileStream("blabla.png", fileStream);
            }

            //read the file
            using (var bytes = spConnector.GetFileStream("blabla.png"))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test 
        }

        /// <summary>
        /// Save file to specified container
        /// </summary>
        [TestMethod]
        public void SharePointConnectorSaveStream2Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                byteCount = fileStream.Length;
                spConnector.SaveFileStream("blabla.png", String.Format("{0}/sub1/sub11/newfolder", testContainerSecure), fileStream);
            }

            //read the file
            using (var bytes = spConnector.GetFileStream("blabla.png", String.Format("{0}/sub1/sub11/newfolder", testContainerSecure)))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test 
        }

        /// <summary>
        /// Save file to specified container, check if overwrite works
        /// </summary>
        [TestMethod]
        public void SharePointConnectorSaveStream3Test()
        {
            // first save
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                spConnector.SaveFileStream("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure), fileStream);
            }

            // overwrite file
            long byteCount = 0;
            using (var fileStream = System.IO.File.OpenRead(@".\resources\custombg.jpg"))
            {
                byteCount = fileStream.Length;
                spConnector.SaveFileStream("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure), fileStream);
            }

            //read the file
            using (var bytes = spConnector.GetFileStream("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure)))
            {
                Assert.IsTrue(byteCount == bytes.Length);
            }

            // file will be deleted at end of test 
        }

        /// <summary>
        /// Delete file from default container
        /// </summary>
        [TestMethod]
        public void SharePointConnectorDelete1Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);

            // upload file
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                spConnector.SaveFileStream("blabla.png", fileStream);
            }

            // delete the file
            spConnector.DeleteFile("blabla.png");

            // read the file
            using (var bytes = spConnector.GetFileStream("blabla.png"))
            {
                Assert.IsNull(bytes);
            }

            // file will be deleted at end of test 
        }

        /// <summary>
        /// Delete file from specific container
        /// </summary>
        [TestMethod]
        public void SharePointConnectorDelete2Test()
        {
            SharePointConnector spConnector = new SharePointConnector(TestCommon.CreateClientContext(), TestCommon.DevSiteUrl, testContainer);

            // upload file
            using (var fileStream = System.IO.File.OpenRead(@".\resources\office365.png"))
            {
                spConnector.SaveFileStream("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure), fileStream);
            }

            // delete the file
            spConnector.DeleteFile("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure));

            // read the file
            using (var bytes = spConnector.GetFileStream("blabla.png", String.Format("{0}/sub1/sub11", testContainerSecure)))
            {
                Assert.IsNull(bytes);
            }

            // file will be deleted at end of test 
        }
        #endregion
    }
}
