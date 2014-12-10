using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Utilities;
using System.Collections.Generic;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class FileFolderExtensionsTests
    {
        private ClientContext clientContext;
        private List documentLibrary;
        private Folder folder;
        private Folder ensureSiteFolderTest;
        private Folder ensureLibraryFolderTest;
        private File file;

        private string DocumentLibraryName = "Unit_Test_Library";
        private string FolderName = "Unit_Test_Folder";
        private string TestFilePath1 = "..\\..\\Resources\\office365.png";
        private string TestFilePath2 = "..\\..\\Resources\\custombg.jpg";
        private string commentText = "Unit_Test_Comment";
        private CheckinType checkInType = CheckinType.MajorCheckIn;

        [TestInitialize()]
        public void Initialize()
        {
            clientContext = TestCommon.CreateClientContext();

            documentLibrary = clientContext.Web.CreateList(ListTemplateType.DocumentLibrary, DocumentLibraryName, false);

            folder = documentLibrary.RootFolder.CreateFolder(FolderName);

            var fci = new FileCreationInformation();
            fci.Content = System.IO.File.ReadAllBytes(TestFilePath1);
            fci.Url = folder.ServerRelativeUrl + "/office365.png";
            fci.Overwrite = true;

            file = folder.Files.Add(fci);

            clientContext.Load(file);

            clientContext.ExecuteQuery();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            //Remove the created test folder
            if (ensureSiteFolderTest != null)
            {
                ensureSiteFolderTest.DeleteObject();
                ensureSiteFolderTest.Context.ExecuteQuery();
            }

            //Remove test library - will also remove folders created in the library
            documentLibrary.DeleteObject();
            clientContext.ExecuteQuery();
            clientContext.Dispose();
        }

        public TestContext TestContext { get; set; }

        [TestMethod()]
        public void CheckOutFileTest()
        {
            clientContext.Web.CheckInFile(file.ServerRelativeUrl, checkInType, commentText);

            clientContext.Web.CheckOutFile(file.ServerRelativeUrl);

            File newFile = clientContext.Web.GetFileByServerRelativeUrl(file.ServerRelativeUrl);

            clientContext.Load(newFile, f => f.CheckOutType);

            clientContext.ExecuteQuery();

            Assert.AreNotEqual(newFile.CheckOutType, CheckOutType.None);
            Assert.AreEqual(newFile.CheckOutType, CheckOutType.Online);

        }

        [TestMethod()]
        public void CheckInFileTest()
        {

            clientContext.Web.CheckOutFile(file.ServerRelativeUrl);

            clientContext.Web.CheckInFile(file.ServerRelativeUrl, checkInType, commentText);

            File newFile = clientContext.Web.GetFileByServerRelativeUrl(file.ServerRelativeUrl);

            clientContext.Load(newFile, f => f.CheckInComment, f => f.Level);

            clientContext.ExecuteQuery();

            Assert.AreEqual(newFile.CheckInComment, commentText);
            Assert.AreEqual(newFile.Level, FileLevel.Published);

        }

        [TestMethod]
        public void EnsureSiteFolderTest()
        {
            string folderName = "test_1";
            clientContext.Web.EnsureFolder(folderName);

            clientContext.Load(clientContext.Web.Folders);
            clientContext.ExecuteQuery();
            ensureSiteFolderTest = null;
            foreach (Folder existingFolder in clientContext.Web.Folders)
            {
                if (string.Equals(existingFolder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ensureSiteFolderTest = existingFolder;
                    break;
                }
            }

            Assert.AreEqual(ensureSiteFolderTest.Name, folderName);
        }

        [TestMethod]
        public void EnsureLibraryFolderTest()
        {
            string folderName = "test_1";

            clientContext.Load(documentLibrary.RootFolder);
            clientContext.ExecuteQuery();
            documentLibrary.RootFolder.EnsureFolder(folderName);

            clientContext.Load(documentLibrary.RootFolder);
            clientContext.ExecuteQuery();
            ensureLibraryFolderTest = null;
            foreach (Folder existingFolder in documentLibrary.RootFolder.Folders)
            {
                if (string.Equals(existingFolder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    ensureLibraryFolderTest = existingFolder;
                    break;
                }
            }

            Assert.AreEqual(ensureLibraryFolderTest.Name, folderName);
        }

        [TestMethod]
        public void EnsureLibraryFolderRecursiveTest()
        {
            string folderName = "test_2/test_22/test_222";

            clientContext.Load(documentLibrary.RootFolder);
            clientContext.ExecuteQuery();
            clientContext.Web.EnsureFolder(documentLibrary.RootFolder, folderName);

            Folder testFolder = clientContext.Web.GetFolderByServerRelativeUrl(String.Format("{0}/{1}", DocumentLibraryName, folderName));
            Assert.IsNotNull(testFolder);

            clientContext.Load(testFolder);
            Utility.EnsureWeb(clientContext.Web.Context, clientContext.Web, "ServerRelativeUrl");
            clientContext.ExecuteQuery();
            Assert.AreEqual(testFolder.ServerRelativeUrl, String.Format("{0}/{1}/{2}",clientContext.Web.ServerRelativeUrl, DocumentLibraryName, folderName));
        }

        [TestMethod]
        public void UploadFileTest() {
            var fileNameExpected = "TestFile1.png";
            var file = folder.UploadFile(fileNameExpected, TestFilePath1, true);

            Assert.AreEqual(fileNameExpected, file.Name);
        }

        [TestMethod]
        public void UploadFileWebDavTest() {
            var fileNameExpected = "TestFile1.png";
            var file = folder.UploadFileWebDav(fileNameExpected, TestFilePath1, true);

            Assert.AreEqual(fileNameExpected, file.Name);
        }

        [TestMethod]
        public void VerifyIfUploadRequiredTest() {
            var fileNameExpected = "TestFile1.png";
            var file = folder.UploadFileWebDav(fileNameExpected, TestFilePath1, true);

            var expectedFalse = file.VerifyIfUploadRequired(TestFilePath1);
            var expectedTrue = file.VerifyIfUploadRequired(TestFilePath2);

            Assert.IsFalse(expectedFalse, "Was not able to tell that the files were the SAME.");
            Assert.IsTrue(expectedTrue, "Was not able to tell that the files were DIFFERENT.");
        }

        [TestMethod]
        public void SetFilePropertiesTest() {
            var fileNameExpected = "TestFile1.png";
            var expectedTitle = "Test file 1";
            var file = folder.UploadFileWebDav(fileNameExpected, TestFilePath1, true);

            var properties = new Dictionary<string,string>();
            properties["Title"] = expectedTitle;
            file.SetFileProperties(properties);

            file.Context.Load(file.ListItemAllFields);
            file.Context.ExecuteQuery();

            var actualTitle = file.ListItemAllFields["Title"];
            Assert.AreEqual(expectedTitle, actualTitle);
        }
    }
}
