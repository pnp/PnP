using Newtonsoft.Json;
using OfficeDevPnP.MSGraphAPIDemo.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace OfficeDevPnP.MSGraphAPIDemo.Components
{
    public static class FilesHelper
    {
        /// <summary>
        /// This method returns the personal drive of the current user
        /// </summary>
        /// <returns>The current user's personal drive</returns>
        public static Drive GetUserPersonalDrive()
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/drive",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri));

            var drive = JsonConvert.DeserializeObject<Drive>(jsonResponse);
            return (drive);
        }

        /// <summary>
        /// This method returns the root folder of the personal drive of the current user
        /// </summary>
        /// <returns>The root folder of the current user's personal drive</returns>
        public static DriveItem GetUserPersonalDriveRoot()
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/drive/root",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri));

            var folder = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
            return (folder);
        }

        /// <summary>
        /// This method returns the children items of a specific folder
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="folderId">The ID of the target folder</param>
        /// <param name="numberOfItems">The number of items to retrieve</param>
        /// <returns>The children items</returns>
        public static List<DriveItem> ListFolderChildren(String driveId, String folderId, Int32 numberOfItems = 100)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}drives/{1}/items/{2}/children?$top={3}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    folderId,
                    numberOfItems));

            var driveItems = JsonConvert.DeserializeObject<DriveItemList>(jsonResponse);
            return (driveItems.DriveItems);
        }

        /// <summary>
        /// This method returns a specific file by ID
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <returns>The file object</returns>
        public static DriveItem GetFile(String driveId, String fileId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}drives/{1}/items/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId));

            var driveItem = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
            return (driveItem);
        }

        /// <summary>
        /// This method returns the content of a specific file by ID
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <returns>The content of the file as a Stream</returns>
        public static Stream GetFileContent(String driveId, String fileId, String contentType)
        {
            Stream fileContent = MicrosoftGraphHelper.MakeGetRequestForStream(
                String.Format("{0}drives/{1}/items/{2}/content",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId),
                    contentType);

            return (fileContent);
        }

        /// <summary>
        /// This method searches for a file in the target drive and optional target folder
        /// </summary>
        /// <param name="searchText">The text to search for</param>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="folderId">The ID of the target folder, optional</param>
        /// <returns>The list of resulting DriveItem objects, if any</returns>
        public static List<DriveItem> Search(String searchText, String driveId, String folderId = null)
        {
            String requestUri = null;
            if (!String.IsNullOrEmpty(folderId))
            {
                requestUri = String.Format("{0}drives/{1}/items/{2}/microsoft.graph.search(q='{3}')",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId, folderId, searchText);
            }
            else
            {
                requestUri = String.Format("{0}drives/{1}/root/microsoft.graph.search(q='{2}')",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId, searchText);
            }

            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(requestUri);

            var driveItems = JsonConvert.DeserializeObject<DriveItemList>(jsonResponse);
            return (driveItems.DriveItems);
        }

        /// <summary>
        /// This method returns the thumbnails of a specific file by ID
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <returns>The file thumbnails for the specific file</returns>
        public static ThumbnailSet GetFileThumbnails(String driveId, String fileId)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}drives/{1}/items/{2}/thumbnails",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId));

            var thumbnails = JsonConvert.DeserializeObject<ThumbnailSetResponse>(jsonResponse);
            return (thumbnails.Value.Count > 0 ? thumbnails.Value[0] : null);
        }

        /// <summary>
        /// This method returns a thumbnail by size of a specific file by ID
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <param name="size">The size of the target thumbnail</param>
        /// <returns>The file thumbnails for the specific file</returns>
        public static Thumbnail GetFileThumbnail(String driveId, String fileId, ThumbnailSize size)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}drives/{1}/items/{2}/thumbnails/0/{3}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId,
                    size.ToString().ToLower()));

            var thumbnail = JsonConvert.DeserializeObject<Thumbnail>(jsonResponse);
            return (thumbnail);
        }

        /// <summary>
        /// This method returns the thumbnails of a specific file by ID
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <returns>The file thumbnails for the specific file</returns>
        public static Stream GetFileThumbnailImage(String driveId, String fileId, ThumbnailSize size)
        {
            String jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}drives/{1}/items/{2}/thumbnails/0/{3}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId,
                    size.ToString().ToLower()));

            var thumbnail = JsonConvert.DeserializeObject<Thumbnail>(jsonResponse);

            var thumbnailImageStream = MicrosoftGraphHelper.MakeGetRequestForStream(
                thumbnail.Url,
                "image/jpeg");

            return (thumbnailImageStream);
        }

        /// <summary>
        /// This method creates a new folder in OneDrive for Business
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="parentFolderId">The ID of the parent folder</param>
        /// <param name="folder">The new folder object to create</param>
        /// <returns>The just created folder</returns>
        public static DriveItem CreateFolder(String driveId, String parentFolderId, DriveItem folder)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}drives/{1}/items/{2}/children",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    parentFolderId),
                    folder, 
                    "application/json");

            var newFolder = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
            return (newFolder);
        }

        /// <summary>
        /// This method creates and uploads a file into a parent folder
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="parentFolderId">The ID of the parent folder</param>
        /// <param name="file">The file object</param>
        /// <param name="content">The binary stream of the file content</param>
        /// <param name="contentType">The content type of the file</param>
        /// <returns>The just created and uploaded file object</returns>
        public static DriveItem UploadFile(String driveId, String parentFolderId, 
            DriveItem file, Stream content, String contentType)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}drives/{1}/items/{2}/children",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    parentFolderId),
                    file,
                    "application/json");

            var uploadedFile = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);

            try
            {
                MicrosoftGraphHelper.MakePutRequest(
                    String.Format("{0}drives/{1}/items/{2}/content",
                        MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                        driveId,
                        uploadedFile.Id),
                        content,
                        contentType);
            }
            catch (ApplicationException ex)
            {
                // For whatever reason we come here ... the upload failed
                // and we need to delete the just created file
                FilesHelper.DeleteFile(driveId, uploadedFile.Id);

                // And then we re-throw the exception
                throw ex;
            }

            return (uploadedFile);
        }

        /// <summary>
        /// This method creates and uploads a file into a parent folder with a unique request
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="parentFolderId">The ID of the parent folder</param>
        /// <param name="file">The file object</param>
        /// <param name="content">The binary stream of the file content</param>
        /// <param name="contentType">The content type of the file</param>
        /// <returns>The just created and uploaded file object</returns>
        public static DriveItem UploadFileDirect(String driveId, String parentFolderId,
            DriveItem file, Stream content, String contentType)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePutRequestForString(
                String.Format("{0}drives/{1}/items/{2}/children/{3}/content",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    parentFolderId,
                    file.Name),
                    content,
                    contentType);

            var uploadedFile = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);

            return (uploadedFile);
        }

        /// <summary>
        /// This method deletes a file in OneDrive for Business
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        public static void DeleteFile(String driveId, String fileId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}drives/{1}/items/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId));
        }

        /// <summary>
        /// This method renames an already existing file in OneDrive for Business
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <param name="newFileName">The new file name</param>
        /// <returns>The updated DriveItem corresponding to the renamed file</returns>
        public static DriveItem RenameFile(String driveId, String fileId, String newFileName)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}drives/{1}/items/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId),
                    new DriveItem {
                        Name = newFileName,
                    },
                    "application/json");

            var updatedFile = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
            return (updatedFile);
        }

        /// <summary>
        /// Uploads a new file content on top of an already existing file
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="fileId">The ID of the target file</param>
        /// <param name="content">The binary stream of the file content</param>
        /// <param name="contentType">The content type of the file</param>
        public static void UpdateFileContent(String driveId, String fileId, 
            Stream content, String contentType)
        {
            MicrosoftGraphHelper.MakePutRequest(
                String.Format("{0}drives/{1}/items/{2}/content",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    fileId),
                    content,
                    contentType);
        }

        /// <summary>
        /// This method moves one item from one parent folder to another
        /// </summary>
        /// <param name="driveId">The ID of the target drive</param>
        /// <param name="driveItemId">The ID of the target file</param>
        /// <param name="newItemName">The new name for the item in the target folder</param>
        /// <param name="newParent">The name of the new target folder</param>
        /// <returns>The moved DriveItem instance</returns>
        public static DriveItem MoveDriveItem(String driveId, String driveItemId, String newItemName, String newParent)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePatchRequestForString(
                String.Format("{0}drives/{1}/items/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveId,
                    driveItemId),
                    new DriveItem
                    {
                        Name = newItemName,
                        ParentReference = new ItemReference
                        {
                            Path = $"/drive/root:/{newParent}"
                        }
                    },
                    "application/json");

            var movedItem = JsonConvert.DeserializeObject<DriveItem>(jsonResponse);
            return (movedItem);
        }

        /// <summary>
        /// This method returns a list of permissions for a specific DriveItem in OneDrive for Business
        /// </summary>
        /// <param name="driveItemId">The ID of the DriveItem</param>
        /// <returns>The list of permissions for the object</returns>
        public static List<Permission> GetDriveItemPermissions(String driveItemId)
        {
            var jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/drive/items/{1}/permissions",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveItemId));

            var permissions = JsonConvert.DeserializeObject<PermissionList>(jsonResponse);
            return (permissions.Permissions);
        }

        /// <summary>
        /// This method returns a permission of a specific DriveItem in OneDrive for Business
        /// </summary>
        /// <param name="driveItemId">The ID of the DriveItem</param>
        /// <param name="permissionId">The ID of the permission</param>
        /// <returns>The permission object</returns>
        public static Permission GetDriveItemPermission(String driveItemId, String permissionId)
        {
            var jsonResponse = MicrosoftGraphHelper.MakeGetRequestForString(
                String.Format("{0}me/drive/items/{1}/permissions/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveItemId,
                    permissionId));

            var permission = JsonConvert.DeserializeObject<Permission>(jsonResponse);
            return (permission);
        }

        /// <summary>
        /// This method removes a permission from a target DriveItem
        /// </summary>
        /// <param name="driveItemId">The ID of the DriveItem</param>
        /// <param name="permissionId">The ID of the permission</param>
        public static void RemoveDriveItemPermission(String driveItemId, String permissionId)
        {
            MicrosoftGraphHelper.MakeDeleteRequest(
                String.Format("{0}me/drive/items/{1}/permissions/{2}",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveItemId,
                    permissionId));
        }

        /// <summary>
        /// This method creates a sharing link for a target DriveItem
        /// </summary>
        /// <param name="driveItemId">The ID of the DriveItem</param>
        /// <param name="type">The type of the sharing link</param>
        /// <param name="scope">The scope of the sharing link</param>
        /// <returns>The just added permission for the sharing link</returns>
        public static Permission CreateSharingLink(String driveItemId, SharingLinkType type, SharingLinkScope scope)
        {
            var jsonResponse = MicrosoftGraphHelper.MakePostRequestForString(
                String.Format("{0}me/drive/items/{1}/createLink",
                    MicrosoftGraphHelper.MicrosoftGraphV1BaseUri,
                    driveItemId),
                    new {
                        @type = type.ToString().ToLower(),
                        @scope = scope.ToString().ToLower(), 
                    },
                    "application/json"
                );

            var addedPermission = JsonConvert.DeserializeObject<Permission>(jsonResponse);
            return (addedPermission);
        }
    }
}