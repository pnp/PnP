using OfficeDevPnP.Core;
using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client.DocumentSet;
using System.ComponentModel;
using System.Security.Cryptography;

namespace Microsoft.SharePoint.Client
{
    public static partial class FileFolderExtensions
    {

        [Obsolete("Use FolderExists() instead, which works for both web sites and subfolders.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SubFolderExists(this Folder folder, string folderName)
        {
            return folder.FolderExists(folderName);
        }

        [Obsolete("Prefer list.RootFolder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadDocumentToLibrary(this Web web, string filePath, string libraryName, bool createLibrary = false)
        {

            if (!web.ListExists(libraryName))
            {
                if (createLibrary)
                {
                    web.AddDocumentLibrary(libraryName);
                }
                else
                {
                    LoggingUtility.Internal.TraceError((int)EventId.LibraryMissing, CoreResources.FileFolderExtensions_LibraryMissing, web.Url, libraryName);
                    // have to abort, list does not exist.
                    string errorMessage = string.Format(CoreResources.FileFolderExtensions_LibraryMissing, web.Url, libraryName);
                    throw new WebException(errorMessage);
                }
            }

            var list = web.Lists.GetByTitle(libraryName);
            list.RootFolder.UploadFile(filePath);
        }

        [Obsolete("Use list.RootFolder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadDocumentToLibrary(this Web web, string filePath, List library)
        {
            var file = library.RootFolder.UploadFile(filePath);
        }

        [Obsolete("Use list.RootFolder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadDocumentToLibrary(this List list, string filePath)
        {
            var file = list.RootFolder.UploadFile(filePath);
        }

        [Obsolete("Prefer web.RootFolder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadDocumentToFolder(this Web web, string filePath, string folderName, bool createFolder = false)
        {
            var filename = Path.GetFileName(filePath);
            LoggingUtility.Internal.TraceInformation((int)EventId.UploadFile, CoreResources.FileFolderExtensions_UploadFile0ToFolder1, filename, folderName);

            Folder folder;
            if (!DoesFolderExists(web, folderName))
            {
                if (createFolder)
                {
                    folder = web.Folders.Add(folderName);
                }
                else
                {
                    // have to abort, list does not exist.
                    string errorMessage = string.Format(CoreResources.FileFolderExtensions_FolderMissing, web.Url, folderName);
                    LoggingUtility.Internal.TraceError((int)EventId.FolderMissing, errorMessage);
                    throw new WebException(errorMessage);
                }
            }

            // Upload document to the folder
            var destinationFolder = web.Folders.GetByUrl(folderName);
            destinationFolder.UploadFile(filePath);
        }

        [Obsolete("Use list.RootFolder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadDocumentToFolder(this Web web, string filePath, Folder folder)
        {
            folder.UploadFile(filePath);
        }

        [Obsolete("Prefer folder.UploadFile() instead.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static void UploadFileToServerRelativeUrl(this Web web, string filePath, string serverRelativeUrl, bool useWebDav = false)
        {
            if (!serverRelativeUrl.ToLower().EndsWith(System.IO.Path.GetFileName(filePath).ToLower()))
            {
                serverRelativeUrl = UrlUtility.Combine(serverRelativeUrl, filePath);
            }
            var clientContext = web.Context as ClientContext;
            if (useWebDav)
            {
                using (FileStream fs = new FileStream(filePath, FileMode.Open))
                {
                    clientContext.ExecuteQuery();
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, serverRelativeUrl, fs, true);
                }
            }
            else
            {
                var files = web.RootFolder.Files;
                clientContext.Load(files);

                clientContext.ExecuteQuery();

                if (files != null)
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                    {
                        FileCreationInformation createInfo = new FileCreationInformation();
                        createInfo.ContentStream = stream;

                        createInfo.Overwrite = true;
                        createInfo.Url = serverRelativeUrl;
                        files.Add(createInfo);
                        clientContext.ExecuteQuery();
                    }
                }
            }
        }

    }
}
