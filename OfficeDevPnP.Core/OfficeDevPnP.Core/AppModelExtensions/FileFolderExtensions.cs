using OfficeAMS.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    public static class FileFolderExtensions
    {
        /// <summary>
        /// Upload document to library
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="filePath">Path to source location like c:\fuu.txt</param>
        /// <param name="libraryName">Name of the document library</param>
        /// <param name="createLibrary">Should library be created if it's not present</param>
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
                    // have to abort, list does not exist.
                    string errorMessage = string.Format("Target list does not exist in the web. Web: {0}, List: {1}", web.Url, libraryName);
                    LoggingUtility.LogError(errorMessage, null, EventCategory.Unknown);
                    throw new WebException(errorMessage);
                }
            }

            UploadDocumentToLibrary(web.Lists.GetByTitle(libraryName), filePath);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="filePath">Path to source location like c:\fuu.txt</param>
        /// <param name="library">Target library where the file is uploaded</param>
        public static void UploadDocumentToLibrary(this Web web, string filePath, List library)
        {
            UploadDocumentToLibrary(library, filePath);
        }

        /// <summary>
        /// Uplaod file to library 
        /// </summary>
        /// <param name="list">List to be processed - can be root web or sub site</param>
        /// <param name="filePath">Path to source location like c:\fuu.txt</param>
        public static void UploadDocumentToLibrary(this List list, string filePath)
        {
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                FileCreationInformation flciNewFile = new FileCreationInformation();

                // This is the key difference for the first case - using ContentStream property
                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(filePath);
                flciNewFile.Overwrite = true;

                Microsoft.SharePoint.Client.File uploadFile = list.RootFolder.Files.Add(flciNewFile);

                list.Context.Load(uploadFile);
                list.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Upload document to folder
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="filePath">Full path to the file like c:\temp\fuu.txt</param>
        /// <param name="folderName">Folder Name in the site</param>
        /// <param name="createLibrary">Should folder be created, if it does not exist</param>
        public static void UploadDocumentToFolder(this Web web, string filePath, string folderName, bool createLibrary = false)
        {
            Folder folder;
            if (!DoesFolderExists(web, folderName))
            {
                if (createLibrary)
                {
                    folder = web.Folders.Add(folderName);
                }
                else
                {
                    // have to abort, list does not exist.
                    string errorMessage = string.Format("Target folder does not exist in the web. Web: {0}, Folder: {1}", web.Url, folderName);
                    LoggingUtility.LogError(errorMessage, null, EventCategory.Unknown);
                    throw new WebException(errorMessage);
                }
            }

            // Upload document to the folder
            UploadDocumentToFolder(web, filePath, web.Folders.GetByUrl(folderName));
        }

        /// <summary>
        /// Upload document to folder
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="filePath">Full path to the file like c:\temp\fuu.txt</param>
        /// <param name="folder">Folder Name in the site</param>
        public static void UploadDocumentToFolder(this Web web, string filePath, Folder folder)
        {
            if (!folder.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(folder);
                web.Context.ExecuteQuery();
            }
            
            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                FileCreationInformation flciNewFile = new FileCreationInformation();

                // This is the key difference for the first case - using ContentStream property
                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(filePath);
                flciNewFile.Overwrite = true;

                Microsoft.SharePoint.Client.File uploadFile = folder.Files.Add(flciNewFile);

                folder.Context.Load(uploadFile);
                folder.Context.ExecuteQuery();
            }

        }

        /// <summary>
        /// Create folder to web
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="folderUrl">Folder URL to be created</param>
        /// <returns></returns>
        public static Folder CreateFolder(this Web web, string folderUrl)
        {
            Folder folder;

            if (!DoesFolderExists(web, folderUrl))
            {
                folder = web.Folders.Add(folderUrl);
            }
            else
            {
                folder = web.Folders.GetByUrl(folderUrl);
            }

            // Load Folder instance
            web.Context.Load(folder);
            web.Context.ExecuteQuery();
            return folder;
        }

        /// <summary>
        /// Checks if a specific folder exists
        /// </summary>
        /// <param name="clientContext">Current User Context</param>
        /// <param name="targetFolderUrl">Folder to check</param>
        /// <returns></returns>
        public static bool DoesFolderExists(this Web web, string targetFolderUrl)
        {
            Folder folder = web.GetFolderByServerRelativeUrl(targetFolderUrl);
            web.Context.Load(folder);
            bool exists = false;

            try
            {
                web.Context.ExecuteQuery();
                exists = true;
            }
            catch (Exception ex)
            {
                return false;
            }

            return exists;
        }

        public static Folder ResolveSubFolder(this Folder folder, string folderName)
        {
            folder.Context.Load(folder);
            folder.Context.Load(folder.Folders);
            folder.Context.ExecuteQuery();
            foreach (Folder subFolder in folder.Folders)
            {
                if (subFolder.Name.ToLowerInvariant() == folderName.ToLowerInvariant())
                {
                    return subFolder;
                }
            }
            return folder;
        }

        public static bool SubFolderExists(this Folder folder, string folderName)
        {
            folder.Context.Load(folder);
            folder.Context.Load(folder.Folders);
            folder.Context.ExecuteQuery();
            foreach (Folder subFolder in folder.Folders)
            {
                if (subFolder.Name.ToLowerInvariant() == folderName.ToLowerInvariant())
                {
                    return true;
                }
            }
            return false;
        }
    }
}
