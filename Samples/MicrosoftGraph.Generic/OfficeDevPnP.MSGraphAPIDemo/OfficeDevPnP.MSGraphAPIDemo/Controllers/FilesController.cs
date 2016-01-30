using OfficeDevPnP.MSGraphAPIDemo.Components;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web;
using System.Web.Mvc;

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

            StringBuilder sb = new StringBuilder();
            String oneFolderId = null;

            foreach (var item in childrenItems)
            {
                if (item.folder != null)
                {
                    sb.AppendFormat("Found folder {0} with {1} child items.\n", item.Name, item.folder.ChildCount);
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

            var searchResult = FilesHelper.Search("Microsoft-Graph-API", drive.Id);

            if (!String.IsNullOrEmpty(oneFolderId))
            {
                var oneFolderChildren = FilesHelper.ListFolderChildren(drive.Id, oneFolderId);
                var file = oneFolderChildren.FirstOrDefault(f => f.Name == "PnP Web Cast - Microsoft-Graph-API.pptx");

                if (file != null)
                {
                    String pptxContentType = "application/vnd.openxmlformats-officedocument.presentationml.presentation";
                    Stream fileContent = FilesHelper.GetFileContent(drive.Id, file.Id, pptxContentType);
                    return (base.File(fileContent, pptxContentType, file.Name));
                }
            }

            return View("Index");
        }
    }
}