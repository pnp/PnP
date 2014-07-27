using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml.Linq;

namespace Contoso.Branding.ApplyBranding
{
    static class BrandingHelper
    {
        public static void UploadFile(ClientContext clientContext, string name, string folder, string path)
        {

            var web = clientContext.Web;
            var filePath = web.ServerRelativeUrl.TrimEnd(Program.trimChars) + "/" + path + "/";

            Console.WriteLine("Uploading file {0} to {1}{2}", name, filePath, folder);

            EnsureFolder(web, filePath, folder);
            CheckOutFile(web, name, filePath, folder);
            var uploadFile = AddFile(web.Url, web, "Branding\\Files\\", name, filePath, folder);
            CheckInPublishAndApproveFile(uploadFile);
        }

        public static void UploadMasterPage(ClientContext clientContext, string name, string folder)
        {
            var web = clientContext.Web;
            var lists = web.Lists;
            var gallery = web.GetCatalog(116);
            clientContext.Load(lists, l => l.Include(ll => ll.DefaultViewUrl));
            clientContext.Load(gallery, g => g.RootFolder.ServerRelativeUrl);
            clientContext.ExecuteQuery();

            Console.WriteLine("Uploading and applying {0} to {1}", name, web.ServerRelativeUrl);

            var masterPath = gallery.RootFolder.ServerRelativeUrl.TrimEnd(new char[] { '/' }) + "/";

            EnsureFolder(web, masterPath, folder);
            CheckOutFile(web, name, masterPath, folder);

            var uploadFile = AddFile(web.Url, web, "Branding\\MasterPages\\", name, masterPath, folder);

            SetMasterPageMetadata(web, uploadFile);
            CheckInPublishAndApproveFile(uploadFile);

            var masterUrl = string.Concat(masterPath, folder, (string.IsNullOrEmpty(folder) ? string.Empty : "/"), name);
            web.CustomMasterUrl = masterUrl;
            web.MasterUrl = masterUrl;
            web.Update();
            clientContext.ExecuteQuery();
        }

        private static void SetMasterPageMetadata(Web web, File uploadFile)
        {
            var parentContentTypeId = "0x010105"; // Master Page
            var gallery = web.GetCatalog(116);
            web.Context.Load(gallery, g => g.ContentTypes);
            web.Context.ExecuteQuery();

            var contentTypeId = gallery.ContentTypes.FirstOrDefault(ct => ct.StringId.StartsWith(parentContentTypeId)).StringId;
            var item = uploadFile.ListItemAllFields;
            web.Context.Load(item);

            item["ContentTypeId"] = contentTypeId;
            item["UIVersion"] = Convert.ToString(15);
            item["MasterPageDescription"] = "Violin master page";
            item.Update();
            web.Context.ExecuteQuery();
        }

        public static void UploadPageLayout(ClientContext clientContext, string name, string folder, string title, string publishingAssociatedContentType)
        {
            var web = clientContext.Web;
            var lists = web.Lists;
            var gallery = web.GetCatalog(116);
            clientContext.Load(lists, l => l.Include(ll => ll.DefaultViewUrl));
            clientContext.Load(gallery, g => g.RootFolder.ServerRelativeUrl);
            clientContext.ExecuteQuery();

            Console.WriteLine("Uploading page layout {0} to {1}", name, clientContext.Web.ServerRelativeUrl);

            var masterPath = gallery.RootFolder.ServerRelativeUrl.TrimEnd(Program.trimChars) + "/";

            EnsureFolder(web, masterPath, folder);
            CheckOutFile(web, name, masterPath, folder);

            var uploadFile = AddFile(web.Url, web, "Branding\\PageLayouts\\", name, masterPath, folder);

            SetPageLayoutMetadata(web, uploadFile, title, publishingAssociatedContentType);
            CheckInPublishAndApproveFile(uploadFile);
        }

        private static void SetPageLayoutMetadata(Web web, File uploadFile, string title, string publishingAssociatedContentType)
        {
            var parentContentTypeId = "0x01010007FF3E057FA8AB4AA42FCB67B453FFC100E214EEE741181F4E9F7ACC43278EE811"; //Page Layout
            var gallery = web.GetCatalog(116);
            web.Context.Load(gallery, g => g.ContentTypes);
            web.Context.ExecuteQuery();

            var contentTypeId = gallery.ContentTypes.FirstOrDefault(ct => ct.StringId.StartsWith(parentContentTypeId)).StringId;
            var item = uploadFile.ListItemAllFields;
            web.Context.Load(item);

            item["ContentTypeId"] = contentTypeId;
            item["Title"] = title;
            item["PublishingAssociatedContentType"] = publishingAssociatedContentType;

            item.Update();
            web.Context.ExecuteQuery();
        }

        private static File AddFile(string rootUrl, Web web, string filePath, string fileName, string serverPath, string serverFolder)
        {
            var fileUrl = string.Concat(serverPath, serverFolder, (string.IsNullOrEmpty(serverFolder) ? string.Empty : "/"), fileName);
            var folder = web.GetFolderByServerRelativeUrl(string.Concat(serverPath, serverFolder));

            FileCreationInformation spFile = new FileCreationInformation()
            {
                Content = System.IO.File.ReadAllBytes(filePath + fileName),
                Url = fileUrl,
                Overwrite = true
            };

            var uploadFile = folder.Files.Add(spFile);
            web.Context.Load(uploadFile, f => f.CheckOutType, f => f.Level);
            web.Context.ExecuteQuery();

            return uploadFile;
        }

        private static void EnsureFolder(Web web, string filePath, string fileFolder)
        {
            if (string.IsNullOrEmpty(fileFolder))
            {
                return;
            }

            var lists = web.Lists;
            web.Context.Load(web);
            web.Context.Load(lists, l => l.Include(ll => ll.DefaultViewUrl));
            web.Context.ExecuteQuery();

            ExceptionHandlingScope scope = new ExceptionHandlingScope(web.Context);
            using (scope.StartScope())
            {
                using (scope.StartTry())
                {
                    var folder = web.GetFolderByServerRelativeUrl(string.Concat(filePath, fileFolder));
                    web.Context.Load(folder);
                }

                using (scope.StartCatch())
                {
                    var list = lists.Where(l => l.DefaultViewUrl.IndexOf(filePath, StringComparison.CurrentCultureIgnoreCase) >= 0).FirstOrDefault();

                    ListItemCreationInformation newFolder = new ListItemCreationInformation();
                    newFolder.UnderlyingObjectType = FileSystemObjectType.Folder;
                    newFolder.FolderUrl = filePath.TrimEnd(Program.trimChars);
                    newFolder.LeafName = fileFolder;

                    ListItem item = list.AddItem(newFolder);
                    web.Context.Load(item);
                    item.Update();
                }

                using (scope.StartFinally())
                {
                    var folder = web.GetFolderByServerRelativeUrl(string.Concat(filePath, fileFolder));
                    web.Context.Load(folder);
                }
            }

            web.Context.ExecuteQuery();
        }

        private static void CheckInPublishAndApproveFile(File uploadFile)
        {
            if (uploadFile.CheckOutType != CheckOutType.None)
            {
                uploadFile.CheckIn("Updating branding", CheckinType.MajorCheckIn);
            }

            if (uploadFile.Level == FileLevel.Draft)
            {
                uploadFile.Publish("Updating branding");
            }

            uploadFile.Context.Load(uploadFile, f => f.ListItemAllFields);
            uploadFile.Context.ExecuteQuery();

            if (uploadFile.ListItemAllFields["_ModerationStatus"].ToString() == "2") // SPModerationStatusType.Pending
            {
                uploadFile.Approve("Updating branding");
                uploadFile.Context.ExecuteQuery();
            }
        }

        private static void CheckOutFile(Web web, string fileName, string filePath, string fileFolder)
        {
            var fileUrl = string.Concat(filePath, fileFolder, (string.IsNullOrEmpty(fileFolder) ? string.Empty : "/"), fileName);
            var temp = web.GetFileByServerRelativeUrl(fileUrl);

            web.Context.Load(temp, f => f.Exists);
            web.Context.ExecuteQuery();

            if (temp.Exists)
            {
                web.Context.Load(temp, f => f.CheckOutType);
                web.Context.ExecuteQuery();

                if (temp.CheckOutType != CheckOutType.None)
                {
                    temp.UndoCheckOut();
                }

                temp.CheckOut();
                web.Context.ExecuteQuery();
            }
        }
    }
}
