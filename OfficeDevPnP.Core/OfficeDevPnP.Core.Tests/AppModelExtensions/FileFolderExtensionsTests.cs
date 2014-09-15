using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.Core.Tests.AppModelExtensions
{
    [TestClass]
    public class FileFolderExtensionsTests
    {
        private ClientContext clientContext;
        private List documentLibrary;
        private Folder folder;
        private File file;

        private string DocumentLibraryName = "Unit_Test_Library";
        private string FolderName = "Unit_Test_Folder";
        private string FilePath = "../../Resources/office365.png";
        private string commentText = "Unit_Test_Comment";
        private CheckinType checkInType = CheckinType.MajorCheckIn;

        [TestInitialize()]
        public void Initialize()
        {
            clientContext = TestCommon.CreateClientContext();

            documentLibrary = clientContext.Web.CreateList(ListTemplateType.DocumentLibrary, DocumentLibraryName, false);

            folder = documentLibrary.RootFolder.CreateFolder(FolderName);

            var fci = new FileCreationInformation();
            fci.Content = System.IO.File.ReadAllBytes(FilePath);
            fci.Url = folder.ServerRelativeUrl + "/office365.png";
            fci.Overwrite = true;

            file = folder.Files.Add(fci);

            clientContext.Load(file);

            clientContext.ExecuteQuery();
        }

        [TestCleanup()]
        public void Cleanup()
        {
            documentLibrary.DeleteObject();
            clientContext.ExecuteQuery();
            clientContext.Dispose();
        }

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
    }
}
