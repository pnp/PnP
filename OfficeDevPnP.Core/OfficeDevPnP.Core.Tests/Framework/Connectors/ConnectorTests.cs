using Microsoft.VisualStudio.TestTools.UnitTesting;
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
        #region Azure connector tests
        [TestMethod]
        public void AzureConnectorGetFile1Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector();
            azureConnector.Parameters.Add(AzureStorageConnector.CONNECTIONSTRING, "DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==");
            azureConnector.Parameters.Add(AzureStorageConnector.CONTAINER, "myfilesprivate");

            string file = azureConnector.GetFile("ProvisioningTemplate.xml");
            Assert.IsNotNull(file);

            string file2 = azureConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        [TestMethod]
        public void AzureConnectorGetFile2Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector("DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==", "myfilesprivate");
            
            string file = azureConnector.GetFile("ProvisioningTemplate.xml");
            Assert.IsNotNull(file);

            string file2 = azureConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        [TestMethod]
        public void AzureConnectorGetFiles1Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector("DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==", "myfilesprivate");
            var files = azureConnector.GetFiles();
            Assert.IsTrue(files.Count > 0);
        }

        [TestMethod]
        public void AzureConnectorGetFiles2Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector("DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==", "myfilesprivate");
            var files = azureConnector.GetFiles("myfiles");
            Assert.IsTrue(files.Count > 0);
        }

        [TestMethod]
        public void AzureConnectorGetFileBytes1Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector("DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==", "myfilesprivate");

            using (var bytes = azureConnector.GetFileStream("ProvisioningTemplate.xml"))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = azureConnector.GetFileStream("Idonotexist.xml"))
            {
                Assert.IsNull(bytes2);
            }
        }

        [TestMethod]
        public void AzureConnectorGetFileBytes2Test()
        {
            AzureStorageConnector azureConnector = new AzureStorageConnector("DefaultEndpointsProtocol=https;AccountName=bjansenmsft;AccountKey=bV4r9PE7w7meJXrXq1WJEpTT/TgJJ+ySmPzP5G3QWX/ZibI6FdvC8xGBNuGDUvhbfv3Ij1VrvzFKXy3O81IQTg==", "myfilesprivate");

            using (var bytes = azureConnector.GetFileStream("office365.png", "myfiles"))
            {
                Assert.IsTrue(bytes.Length > 0);
            }

            using (var bytes2 = azureConnector.GetFileStream("Idonotexist.xml", "myfiles"))
            {
                Assert.IsNull(bytes2);
            }
        }
        #endregion

        #region File connector tests
        [TestMethod]
        public void FileConnectorGetFile1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "Templates");

            string file = fileSystemConnector.GetFile("ProvisioningTemplate.xml");
            Assert.IsNotNull(file);

            string file2 = fileSystemConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        [TestMethod]
        public void FileConnectorGetFile2Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@"C:\GitHub\BertPnP\OfficeDevPnP.Core\OfficeDevPnP.Core.Tests\resources", "templates");

            string file = fileSystemConnector.GetFile("ProvisioningTemplate.xml");
            Assert.IsNotNull(file);

            string file2 = fileSystemConnector.GetFile("Idonotexist.xml");
            Assert.IsNull(file2);
        }

        [TestMethod]
        public void FileConnectorGetFiles1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "Templates");
            var files = fileSystemConnector.GetFiles();
            Assert.IsTrue(files.Count > 0);
        }

        [TestMethod]
        public void FileConnectorGetFiles2Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "");
            var files = fileSystemConnector.GetFiles("Templates");
            Assert.IsTrue(files.Count > 0);
        }

        [TestMethod]
        public void FileConnectorGetFileBytes1Test()
        {
            FileSystemConnector fileSystemConnector = new FileSystemConnector(@".\Resources", "Templates");

            using (var bytes = fileSystemConnector.GetFileStream("ProvisioningTemplate.xml"))
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
