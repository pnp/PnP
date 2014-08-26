using OfficeDevPnP.Core.Utilities;
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
                    string errorMessage = string.Format("Target library does not exist in the web. Web: {0}, List: {1}", web.Url, libraryName);
                    LoggingUtility.Internal.TraceError((int)EventId.LibraryMissing, errorMessage);
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
        /// Upload file to library 
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
            var filename = Path.GetFileName(filePath);
            LoggingUtility.Internal.TraceInformation((int)EventId.UploadFile, "Uploading file '{0}' to folder '{1}'.", filename, folderName);

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
                    LoggingUtility.Internal.TraceError((int)EventId.FolderMissing, errorMessage);
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
        [Obsolete("Use EnsureFolder() instead, which works for both web sites and subfolders.")]
        public static Folder CreateFolder(this Web web, string folderName)
        {
            return EnsureFolder(web, folderName);
        }

        /// <summary>
        /// Checks if the folder exists at the top level of the web site, and if it does not exist creates it.
        /// </summary>
        /// <param name="web">Web to check for the named folder</param>
        /// <param name="folderName">Folder name to retrieve or create</param>
        /// <returns>The existing or newly created folder</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// </remarks>
        public static Folder EnsureFolder(this Web web, string folderName)
        {
            var folderCollection = web.Folders;
            var folder = EnsureFolderImplementation(folderCollection, folderName);
            return folder;
        }

        /// <summary>
        /// Checks if the subfolder exists, and if it does not exist creates it.
        /// </summary>
        /// <param name="parentFolder">Parent folder to create under</param>
        /// <param name="folderName">Folder name to retrieve or create</param>
        /// <returns>The existing or newly created folder</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// </remarks>
        public static Folder EnsureFolder(this Folder parentFolder, string folderName)
        {
            var folderCollection = parentFolder.Folders;
            var folder = EnsureFolderImplementation(folderCollection, folderName);
            return folder;
        }

        private static Folder EnsureFolderImplementation(FolderCollection folderCollection, string folderName)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQuery();
            foreach (Folder folder in folderCollection)
            {
                if (string.Equals(folder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return folder;
                }
            }

            var newFolder = folderCollection.Add(folderName);
            folderCollection.Context.Load(newFolder);
            folderCollection.Context.ExecuteQuery();

            return newFolder;
        }

        /// <summary>
        /// Checks if the folder exists at the top level of the web site.
        /// </summary>
        /// <param name="web">Web to check for the named folder</param>
        /// <param name="folderName">Folder name to retrieve</param>
        /// <returns>true if the folder exists; false otherwise</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// </remarks>
        public static bool FolderExists(this Web web, string folderName)
        {
            var folderCollection = web.Folders;
            var exists = FolderExistsImplementation(folderCollection, folderName);
            return exists;
        }

        /// <summary>
        /// Checks if the subfolder exists.
        /// </summary>
        /// <param name="parentFolder">Parent folder to check for the named subfolder</param>
        /// <param name="folderName">Folder name to retrieve</param>
        /// <returns>true if the folder exists; false otherwise</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// </remarks>
        public static bool FolderExists(this Folder parentFolder, string folderName)
        {
            var folderCollection = parentFolder.Folders;
            var exists = FolderExistsImplementation(folderCollection, folderName);
            return exists;
        }

        private static bool FolderExistsImplementation(FolderCollection folderCollection, string folderName)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQuery();
            foreach (Folder folder in folderCollection)
            {
                if (string.Equals(folder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Checks if a specific folder exists
        /// </summary>
        /// <param name="clientContext">Current User Context</param>
        /// <param name="serverRelativeFolderUrl">Folder to check</param>
        /// <returns></returns>
        public static bool DoesFolderExists(this Web web, string serverRelativeFolderUrl)
        {
            Folder folder = web.GetFolderByServerRelativeUrl(serverRelativeFolderUrl);
            web.Context.Load(folder);
            bool exists = false;

            try
            {
                web.Context.ExecuteQuery();
                exists = true;
            }
            catch
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
            return null;
        }

        [Obsolete("Use FolderExists() instead, which works for both web sites and subfolders.")]
        public static bool SubFolderExists(this Folder folder, string folderName)
        {
            return folder.FolderExists(folderName);
        }
    }
}
