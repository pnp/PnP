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

namespace Microsoft.SharePoint.Client
{
    public static class FileFolderExtensions
    {
        /// <summary>
        /// Approves a file
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="serverRelativeUrl">The server relative url of the file to approve</param>
        /// <param name="comment"></param>
        public static void ApproveFile(this Web web, string serverRelativeUrl, string comment)
        {
            File file = null;
            file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQuery();
            if (file.Exists)
            {
                file.Approve(comment);
            }
            web.Context.ExecuteQuery();
        }

        /// <summary>
        /// Checks in a file
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="serverRelativeUrl">The server rrelative url of the file to checkin</param>
        public static void CheckInFile(this Web web, string url, CheckinType checkinType, string comment)
        {
            File file = web.GetFileByServerRelativeUrl(url);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQuery();

            if (file.Exists)
            {
                if (file.CheckOutType != CheckOutType.None)
                {
                    file.CheckIn(comment, checkinType);
                    web.Context.ExecuteQuery();
                }
            }
        }

        /// <summary>
        /// Checks out a file
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="serverRelativeUrl">The server rrelative url of the file to checkout</param>
        public static void CheckOutFile(this Web web, string serverRelativeUrl)
        {
            File file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQuery();

            if (file.Exists)
            {
                if (file.CheckOutType == CheckOutType.None)
                {
                    file.CheckOut();
                    web.Context.ExecuteQuery();
                }
            }
        }

        private static void CopyStream(Stream source, Stream destination)
        {
            byte[] buffer = new byte[32768];
            int bytesRead;
            do
            {
                bytesRead = source.Read(buffer, 0, buffer.Length);
                destination.Write(buffer, 0, bytesRead);
            } while (bytesRead != 0);
        }

        /// <summary>
        /// Creates a new document set as a child of an existing folder, with the specified content type ID.
        /// </summary>
        /// <param name="folder"></param>
        /// <param name="documentSetName"></param>
        /// <param name="contentTypeId">Content type of the document set</param>
        /// <returns>The created Folder representing the document set, so that additional operations (such as setting properties) can be done.</returns>
        /// <remarks>
        /// <example>
        ///     var setContentType = list.BestMatchContentTypeId(BuiltInContentTypeId.DocumentSet);
        ///     var set1 = list.RootFolder.CreateDocumentSet("Set 1", setContentType);
        /// </example>
        /// </remarks>
        public static Folder CreateDocumentSet(this Folder folder, string documentSetName, ContentTypeId contentTypeId)
        {
            if (folder == null) { throw new ArgumentNullException("folder"); }
            if (documentSetName == null) { throw new ArgumentNullException("documentSetName"); }
            if (contentTypeId == null) { throw new ArgumentNullException("contentTypeId"); }
            // TODO: Check for any other illegal characters in SharePoint
            if (documentSetName.Contains('/') || documentSetName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single document set name and cannot contain path characters.", "documentSetName");
            }

            LoggingUtility.Internal.TraceInformation(1, "Creating document set '{0}'.", documentSetName);

            var result = DocumentSet.DocumentSet.Create(folder.Context, folder, documentSetName, contentTypeId);
            folder.Context.ExecuteQuery();

            var fullUri = new Uri(result.Value);
            var serverRelativeUrl = fullUri.AbsolutePath;
            var documentSetFolder = folder.Folders.GetByUrl(serverRelativeUrl);

            return documentSetFolder;
        }

        /// <summary>
        /// Creates a folder with the given name as a child of the Web. 
        /// Note it is more common to create folders within an existing Folder, such as the RootFolder of a List.
        /// </summary>
        /// <param name="web">Web to check for the named folder</param>
        /// <param name="folderName">Folder name to retrieve or create</param>
        /// <returns>The newly created Folder, so that additional operations (such as setting properties) can be done.</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// </remarks>
        public static Folder CreateFolder(this Web web, string folderName)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            var folderCollection = web.Folders;
            var folder = CreateFolderImplementation(folderCollection, folderName);
            return folder;
        }

        /// <summary>
        /// Creates a folder with the given name.
        /// </summary>
        /// <param name="parentFolder">Parent folder to create under</param>
        /// <param name="folderName">Folder name to retrieve or create</param>
        /// <returns>The newly created folder</returns>
        /// <remarks>
        /// <para>
        /// Note that this only checks one level of folder (the Folders collection) and cannot accept a name with path characters.
        /// </para>
        /// <example>
        ///     var folder = list.RootFolder.CreateFolder("new-folder");
        /// </example>
        /// </remarks>
        public static Folder CreateFolder(this Folder parentFolder, string folderName)
        {
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            var folderCollection = parentFolder.Folders;
            var folder = CreateFolderImplementation(folderCollection, folderName);
            return folder;
        }

        private static Folder CreateFolderImplementation(FolderCollection folderCollection, string folderName)
        {
            var newFolder = folderCollection.Add(folderName);
            folderCollection.Context.Load(newFolder);
            folderCollection.Context.ExecuteQuery();

            return newFolder;
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

        /// <summary>
        /// Checks if the folder exists at the top level of the web site, and if it does not exist creates it.
        /// Note it is more common to create folders within an existing Folder, such as the RootFolder of a List.
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
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

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
            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            var folderCollection = parentFolder.Folders;
            var folder = EnsureFolderImplementation(folderCollection, folderName);
            return folder;
        }

        private static Folder EnsureFolderImplementation(FolderCollection folderCollection, string folderName)
        {
            Folder folder = null;

            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQuery();
            foreach (Folder existingFolder in folderCollection)
            {
                if (string.Equals(folder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    folder = existingFolder;
                    break;
                }
            }

            if (folder == null)
            {
                folder = CreateFolderImplementation(folderCollection, folderName);
            }

            return folder;
        }

        /// <summary>
        /// Finds files in the web. Can be slow.
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="match">a wildcard pattern to match</param>
        /// <returns></returns>
        public static List<Microsoft.SharePoint.Client.File> FindFiles(this Web web, string match)
        {
            Folder rootFolder = web.RootFolder;
            match = WildcardToRegex(match);
            List<Microsoft.SharePoint.Client.File> files = new List<Microsoft.SharePoint.Client.File>();

            ParseFiles(rootFolder, match, web.Context as ClientContext, ref files);

            return files;
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
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");

            var folderCollection = parentFolder.Folders;
            var exists = FolderExistsImplementation(folderCollection, folderName);
            return exists;
        }

        private static bool FolderExistsImplementation(FolderCollection folderCollection, string folderName)
        {
            if (folderCollection == null)
                throw new ArgumentNullException("folderCollection");

            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");

            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQuery();
            foreach (Folder folder in folderCollection)
            {
                if (folder.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns a file as string
        /// </summary>
        /// <param name="web">The Web to process</param>
        /// <param name="serverRelativeUrl">The server relative url to the file</param>
        /// <returns></returns>
        public static string GetFileAsString(this Web web, string serverRelativeUrl)
        {
            string returnString = string.Empty;

            var file = web.GetFileByServerRelativeUrl(serverRelativeUrl);

            web.Context.Load(file);

            web.Context.ExecuteQuery();

            ClientResult<Stream> stream = file.OpenBinaryStream();

            web.Context.ExecuteQuery();

            using (Stream memStream = new MemoryStream())
            {
                CopyStream(stream.Value, memStream);

                memStream.Position = 0;

                StreamReader reader = new StreamReader(memStream);

                returnString = reader.ReadToEnd();
            }
            return returnString;
        }

        private static void ParseFiles(Folder folder, string match, ClientContext context, ref List<Microsoft.SharePoint.Client.File> foundFiles)
        {

            FileCollection files = folder.Files;
            context.Load(files, fs => fs.Include(f => f.ServerRelativeUrl, f => f.Name, f => f.Title, f => f.TimeCreated, f => f.TimeLastModified));
            context.Load(folder.Folders);
            context.ExecuteQuery();
            foreach (Microsoft.SharePoint.Client.File file in files)
            {
                if (Regex.IsMatch(file.Name, match, RegexOptions.IgnoreCase))
                {

                    foundFiles.Add(file);
                }
            }
            foreach (Folder subfolder in folder.Folders)
            {
                ParseFiles(subfolder, match, context, ref foundFiles);
            }
        }

        /// <summary>
        /// Publishes a file existing on a server url
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="serverRelativeUrl">the server relative url of the file to publish</param>
        /// <param name="comment"></param>
        public static void PublishFile(this Web web, string serverRelativeUrl, string comment)
        {
            File file = null;
            file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQuery();
            if (file.Exists)
            {
                file.Publish(comment);
            }
            web.Context.ExecuteQuery();
        }
      
        public static Folder ResolveSubFolder(this Folder folder, string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
                throw new ArgumentNullException("folderName");

            folder.Context.Load(folder);
            folder.Context.Load(folder.Folders);
            folder.Context.ExecuteQuery();
            foreach (Folder subFolder in folder.Folders)
            {
                if (subFolder.Name.Equals(folderName, StringComparison.InvariantCultureIgnoreCase))
                {
                    return subFolder;
                }
            }
            return null;
        }

        /// <summary>
        /// Saves a remote file to a local folder
        /// </summary>
        /// <param name="web">The Web to process</param>
        /// <param name="serverRelativeUrl">The server relative url to the file</param>
        /// <param name="localPath">The local folder</param>
        /// <param name="localFileName">The local filename. If null the filename of the file on the server will be used</param>
        public static void SaveFileToLocal(this Web web, string serverRelativeUrl, string localPath, string localFileName = null)
        {
            var clientContext = web.Context as ClientContext;
            var file = web.GetFileByServerRelativeUrl(serverRelativeUrl);

            clientContext.Load(file);

            clientContext.ExecuteQuery();

            ClientResult<Stream> stream = file.OpenBinaryStream();

            clientContext.ExecuteQuery();

            string fileOut;


            if (!string.IsNullOrEmpty(localFileName))
            {
                fileOut = Path.Combine(localPath, localFileName);
            }
            else
            {
                fileOut = Path.Combine(localPath, file.Name);
            }

            using (Stream fileStream = new FileStream(fileOut, FileMode.Create))
            {
                CopyStream(stream.Value, fileStream);
            }
        }

        [Obsolete("Use FolderExists() instead, which works for both web sites and subfolders.")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static bool SubFolderExists(this Folder folder, string folderName)
        {
            return folder.FolderExists(folderName);
        }

        /// <summary>
        /// Upload document to library
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="filePath">Path to source location like c:\fuu.txt</param>
        /// <param name="libraryName">Name of the document library</param>
        /// <param name="createLibrary">Should library be created if it's not present</param>
        [Obsolete("Prefer list.RootFolder.UploadFile() instead.")]
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

        /// <summary>
        /// Upload document to folder within a Web
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="filePath">Full path to the file like c:\temp\fuu.txt</param>
        /// <param name="folderName">Folder Name in the site</param>
        /// <param name="createFolder">Should folder be created, if it does not exist</param>
        [Obsolete("Prefer web.RootFolder.UploadFile() instead.")]
        public static void UploadDocumentToFolder(this Web web, string filePath, string folderName, bool createFolder = false)
        {
            var filename = Path.GetFileName(filePath);
            LoggingUtility.Internal.TraceInformation((int)EventId.UploadFile, CoreResources.FileFolderExtensions_UploadFile, filename, folderName);

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

        /// <summary>
        /// Uploads a file to the specified folder, with an optional content type.
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="filePath">Location of the file to be uploaded.</param>
        /// <param name="contentTypeId">Optional content type; if null (default) the default content type will be used.</param>
        /// <param name="overwriteIfExists">true (default) to overwite existing files</param>
        /// <param name="useWebDav">true (default) to save the binary directly (via webdav); false to use file creation</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static File UploadFile(this Folder folder, string filePath, ContentTypeId contentTypeId = null, bool overwriteIfExists = true, bool useWebDav = true)
        {
            if (filePath == null) { throw new ArgumentNullException("filePath"); }
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException("File path is required.", "filePath"); }

            var fileName = System.IO.Path.GetFileName(filePath);

            LoggingUtility.Internal.TraceInformation(1, "Uploading file '{0}'.", fileName);

            File file = null;
            if (useWebDav)
            {
                if (!folder.IsObjectPropertyInstantiated("ServerRelativeUrl"))
                {
                    folder.Context.Load(folder, f => f.ServerRelativeUrl);
                    folder.Context.ExecuteQuery();
                }
                var serverRelativeUrl = UrlUtility.Combine(folder.ServerRelativeUrl, fileName);
                using (var uploadContext = new ClientContext(folder.Context.Url) { Credentials = folder.Context.Credentials })
                {
                    using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                    {
                        LoggingUtility.Internal.TraceVerbose("Save binary direct (via webdav) to '{0}'", serverRelativeUrl);
                        File.SaveBinaryDirect(uploadContext, serverRelativeUrl, fs, overwriteIfExists);
                        folder.Context.ExecuteQuery();
                    }
                }
                file = folder.Files.GetByUrl(serverRelativeUrl);
            }
            else
            {
                using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open))
                {
                    FileCreationInformation fileCreation = new FileCreationInformation();
                    fileCreation.ContentStream = fs;
                    fileCreation.Url = fileName;
                    fileCreation.Overwrite = overwriteIfExists;
                    LoggingUtility.Internal.TraceVerbose("Creating file info with Url '{0}'", fileCreation.Url);
                    file = folder.Files.Add(fileCreation);
                    folder.Context.ExecuteQuery();
                }
            }

            if (contentTypeId != null)
            {
                LoggingUtility.Internal.TraceVerbose("Setting content type to '{0}'", contentTypeId.StringValue);
                file.ListItemAllFields["ContentTypeId"] = contentTypeId.StringValue;
                file.ListItemAllFields.Update();
            }

            return file;
        }

        /// <summary>
        /// Uploads a file to a server relative url
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="filePath">Full path to the file like c:\temp\fuu.txt</param>
        /// <param name="serverRelativeUrl">Full server relative destination url of the file on the server</param>
        /// <param name="useWebDav">Use webdav uploads, better suitable for larger files.</param>
        [Obsolete("Prefer folder.UploadFile() instead.")]
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

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$";
        }

    }
}
