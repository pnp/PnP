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

        public static List<Permission> GetDriveItemPermissions(String driveItemId)
        {
            return (null);
        }

        public static List<DriveItem> GetDriveItemChildren(String driveItemId)
        {
            return (null);
        }

        public static List<ThumbnailSet> GetDriveItemThumbnails(String driveItemId)
        {
            return (null);
        }
    }
}