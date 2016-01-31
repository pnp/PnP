using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System.Threading;

namespace OfficeDevPnP.MSGraphAPIDemo.Controllers
{
    public class FilesController : Controller
    {
        // GET: Files
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult PlayWithFiles()
        {
            var drive = FilesHelper.GetUserPersonalDrive();
            var root = FilesHelper.GetUserPersonalDriveRoot();
            var childrenItems = FilesHelper.ListFolderChildren(drive.Id, root.Id);

            var newFileOnRoot = UploadSampleFile(drive, root);

            // Collect information about children items in the root folder
            StringBuilder sb = new StringBuilder();
            String oneFolderId = null;

            foreach (var item in childrenItems)
            {
                if (item.Folder != null)
                {
                    sb.AppendFormat("Found folder {0} with {1} child items.\n", item.Name, item.Folder.ChildCount);
                    if (item.Name == "One Folder")
                    {
                        oneFolderId = item.Id;
                    }
                }
                else
                {
                    sb.AppendFormat("Found file {0}.\n", item.Name);
                }
            }
            var filesLog = sb.ToString();

            // Create a new folder in the root folder
            var newFolder = FilesHelper.CreateFolder(drive.Id, root.Id,
                new Models.DriveItem
                {
                    Name = $"Folder Created via API - {DateTime.Now.GetHashCode()}",
                    Folder = new Models.Folder { },
                });

            var newFile = UploadSampleFile(drive, newFolder);
            UpdateSampleFile(drive, newFile);

            // Create another folder in the root folder
            var anotherFolder = FilesHelper.CreateFolder(drive.Id, root.Id,
                new Models.DriveItem
                {
                    Name = $"Folder Created via API - {DateTime.Now.GetHashCode()}",
                    Folder = new Models.Folder { },
                });

            var movedItem = FilesHelper.MoveDriveItem(drive.Id, newFile.Id, "moved.jpg", anotherFolder.Name);
            var movedFolder = FilesHelper.MoveDriveItem(drive.Id, anotherFolder.Id, "Moved Folder", newFolder.Name);

            var searchResult = FilesHelper.Search("PnPLogo", drive.Id, root.Id);

            if (searchResult != null && searchResult.Count > 0)
            {
                var firstFileResult = searchResult.FirstOrDefault(i => i.File != null);

                var thumbnails = FilesHelper.GetFileThumbnails(drive.Id, firstFileResult.Id);
                var thumbnailMedium = FilesHelper.GetFileThumbnail(drive.Id, firstFileResult.Id, Models.ThumbnailSize.Medium);
                var thumbnailImage = FilesHelper.GetFileThumbnailImage(drive.Id, firstFileResult.Id, Models.ThumbnailSize.Medium);
            }

            var permission = FilesHelper.GetDriveItemPermission(newFileOnRoot.Id, "0");

            FilesHelper.DeleteFile(drive.Id, newFileOnRoot.Id);

            var sharingPermission = FilesHelper.CreateSharingLink(newFolder.Id, 
                SharingLinkType.View, SharingLinkScope.Anonymous);

            if (!String.IsNullOrEmpty(oneFolderId))
            {
                var newFolderChildren = FilesHelper.ListFolderChildren(drive.Id, newFolder.Id);
                var file = newFolderChildren.FirstOrDefault(f => f.Name == "moved.jpg");

                if (file != null)
                {
                    String jpegContentType = "image/jpeg";
                    Stream fileContent = FilesHelper.GetFileContent(drive.Id, file.Id, jpegContentType);
                    return (base.File(fileContent, jpegContentType, file.Name));
                }
            }

            return View("Index");
        }

        private static Models.DriveItem UploadSampleFile(Models.Drive drive, Models.DriveItem newFolder)
        {
            Models.DriveItem result = null;
            Stream memPhoto = getFileContent(@"C:\github\PaoloPia-PnP\Samples\MicrosoftGraph.Generic\OfficeDevPnP.MSGraphAPIDemo\OfficeDevPnP.MSGraphAPIDemo\AppIcon.png");

            try
            {
                if (memPhoto.Length > 0)
                {
                    String contentType = "image/png";
                    result = FilesHelper.UploadFile(drive.Id, newFolder.Id,
                        new Models.DriveItem
                        {
                            File = new Models.File { },
                            Name = "PnPLogo.png",
                            ConflictBehavior = "rename",
                        },
                        memPhoto,
                        contentType);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
            }

            return (result);
        }

        private void UpdateSampleFile(Drive drive, DriveItem newFile)
        {
            FilesHelper.RenameFile(drive.Id, newFile.Id, "SP2016-MinRoles.jpg");

            Stream memPhoto = getFileContent(@"C:\github\PaoloPia-PnP\Samples\MicrosoftGraph.Generic\OfficeDevPnP.MSGraphAPIDemo\OfficeDevPnP.MSGraphAPIDemo\SP2016-MinRole.jpg");

            try
            {
                if (memPhoto.Length > 0)
                {
                    String contentType = "image/jpeg";
                    FilesHelper.UpdateFileContent(
                        drive.Id,
                        newFile.Id,
                        memPhoto,
                    contentType);
                }
            }
            catch (Exception ex)
            {
                // Handle the exception
            }
        }

        private static Stream getFileContent(String filePath)
        {
            MemoryStream memPhoto = new MemoryStream();
            using (FileStream fs = new FileStream(filePath, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                Byte[] newPhoto = new Byte[fs.Length];
                fs.Read(newPhoto, 0, (Int32)(fs.Length - 1));
                memPhoto.Write(newPhoto, 0, newPhoto.Length);
                memPhoto.Position = 0;
            }

            return memPhoto;
        }
    }
}