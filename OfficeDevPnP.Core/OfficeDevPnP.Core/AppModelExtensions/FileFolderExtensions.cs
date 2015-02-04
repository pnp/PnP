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
        const string REGEX_INVALID_FILE_NAME_CHARS = @"[<>:;*?/\\|""&%\t\r\n]";

        /// <summary>
        /// Approves a file
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="serverRelativeUrl">The server relative url of the file to approve</param>
        /// <param name="comment">Message to be recorded with the approval</param>
        public static void ApproveFile(this Web web, string serverRelativeUrl, string comment)
        {
            File file = null;
            file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQueryRetry();

            if (file.Exists)
            {
                file.Approve(comment);
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Checks in a file
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="url">The server relative url of the file to checkin</param>
        /// <param name="checkinType">The type of the checkin</param>
        /// <param name="comment">Message to be recorded with the approval</param>
        public static void CheckInFile(this Web web, string url, CheckinType checkinType, string comment)
        {
            File file = web.GetFileByServerRelativeUrl(url);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQueryRetry();

            if (file.Exists)
            {
                if (file.CheckOutType != CheckOutType.None)
                {
                    file.CheckIn(comment, checkinType);
                    web.Context.ExecuteQueryRetry();
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
            web.Context.ExecuteQueryRetry();

            if (file.Exists)
            {
                if (file.CheckOutType == CheckOutType.None)
                {
                    file.CheckOut();
                    web.Context.ExecuteQueryRetry();
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

            LoggingUtility.Internal.TraceInformation(1, CoreResources.FieldAndContentTypeExtensions_CreateDocumentSet, documentSetName);

            var result = DocumentSet.DocumentSet.Create(folder.Context, folder, documentSetName, contentTypeId);
            folder.Context.ExecuteQueryRetry();

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
            folderCollection.Context.ExecuteQueryRetry();

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
                web.Context.ExecuteQueryRetry();
                exists = true;
            }
            catch
            {
                return false;
            }

            return exists;
        }

        /// <summary>
        /// Ensure that the folder structure is created. This also ensures hierarchy of folders.
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="parentFolder">Parent folder</param>
        /// <param name="folderPath">Folder path</param>
        /// <returns>The folder structure</returns>
        public static Folder EnsureFolder(this Web web, Folder parentFolder, string folderPath)
        {
            if (!web.IsPropertyAvailable("ServerRelativeUrl") || !parentFolder.IsPropertyAvailable("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.Load(parentFolder, f => f.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }

            var parentWebRelativeUrl = parentFolder.ServerRelativeUrl.Substring(web.ServerRelativeUrl.Length);
            var webRelativeUrl = parentWebRelativeUrl + (parentWebRelativeUrl.EndsWith("/") ? "" : "/") + folderPath;

            return web.EnsureFolderPath(webRelativeUrl);
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
            folderCollection.Context.ExecuteQueryRetry();
            foreach (Folder existingFolder in folderCollection)
            {
                if (string.Equals(existingFolder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
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
        /// Check if a folder exists with the specified path (relative to the web), and if not creates it (inside a list if necessary)
        /// </summary>
        /// <param name="web">Web to check for the specified folder</param>
        /// <param name="webRelativeUrl">Path to the folder, relative to the web site</param>
        /// <returns>The existing or newly created folder</returns>
        /// <remarks>
        /// <para>
        /// If the specified path is inside an existing list, then the folder is created inside that list.
        /// </para>
        /// <para>
        /// Any existing folders are traversed, and then any remaining parts of the path are created as new folders.
        /// </para>
        /// </remarks>
        public static Folder EnsureFolderPath(this Web web, string webRelativeUrl)
        {
            if (webRelativeUrl == null) { throw new ArgumentNullException("webRelativeUrl"); }
            if (string.IsNullOrWhiteSpace(webRelativeUrl)) { throw new ArgumentException("Folder URL is required.", "webRelativeUrl"); }

            // Check if folder exists
            if (!web.IsObjectPropertyInstantiated("ServerRelativeUrl"))
            {
                web.Context.Load(web, w => w.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            var folderServerRelativeUrl = web.ServerRelativeUrl + (web.ServerRelativeUrl.EndsWith("/") ? "" : "/") + webRelativeUrl;

            // Check if folder is inside a list
            var listCollection = web.Lists;
            web.Context.Load(listCollection, lc => lc.Include(l => l.RootFolder.ServerRelativeUrl));
            web.Context.ExecuteQueryRetry();

            List containingList = null;
            foreach (var list in listCollection)
            {
                if (folderServerRelativeUrl.StartsWith(list.RootFolder.ServerRelativeUrl, StringComparison.InvariantCultureIgnoreCase))
                {
                    containingList = list;
                    break;
                }
            }

            // Start either at the root of the list or web
            string locationType = null;
            string rootUrl = null;
            Folder currentFolder = null;
            if (containingList == null)
            {
                locationType = "Web";
                currentFolder = web.RootFolder;
                web.Context.Load(currentFolder, f => f.ServerRelativeUrl);
                web.Context.ExecuteQueryRetry();
            }
            else
            {
                locationType = "List";
                currentFolder = containingList.RootFolder;
            }

            rootUrl = currentFolder.ServerRelativeUrl;

            // Get remaining parts of the path and split
            var folderRootRelativeUrl = folderServerRelativeUrl.Substring(currentFolder.ServerRelativeUrl.Length);
            var childFolderNames = folderRootRelativeUrl.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            var currentCount = 0;

            foreach (var folderName in childFolderNames)
            {
                currentCount++;

                // Find next part of the path
                var folderCollection = currentFolder.Folders;
                folderCollection.Context.Load(folderCollection);
                folderCollection.Context.ExecuteQueryRetry();
                Folder nextFolder = null;
                foreach (Folder existingFolder in folderCollection)
                {
                    if (string.Equals(existingFolder.Name, folderName, StringComparison.InvariantCultureIgnoreCase))
                    {
                        nextFolder = existingFolder;
                        break;
                    }
                }

                // Or create it
                if (nextFolder == null)
                {
                    var createPath = string.Join("/", childFolderNames, 0, currentCount);
                    LoggingUtility.Internal.TraceInformation((int)EventId.CreateFolder, CoreResources.FileFolderExtensions_CreateFolder0Under12, createPath, locationType, rootUrl);

                    nextFolder = folderCollection.Add(folderName);
                    folderCollection.Context.Load(nextFolder);
                    folderCollection.Context.ExecuteQueryRetry();
                }

                currentFolder = nextFolder;
            }

            return currentFolder;
        }

        /// <summary>
        /// Finds files in the web. Can be slow.
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="match">a wildcard pattern to match</param>
        /// <returns>A list with the found <see cref="Microsoft.SharePoint.Client.File"/> objects</returns>
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
            {
                throw new ArgumentNullException("folderName");
            }

            var folderCollection = parentFolder.Folders;
            var exists = FolderExistsImplementation(folderCollection, folderName);
            return exists;
        }

        private static bool FolderExistsImplementation(FolderCollection folderCollection, string folderName)
        {
            if (folderCollection == null)
            {
                throw new ArgumentNullException("folderCollection");
            }

            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            // TODO: Check for any other illegal characters in SharePoint
            if (folderName.Contains('/') || folderName.Contains('\\'))
            {
                throw new ArgumentException("The argument must be a single folder name and cannot contain path characters.", "folderName");
            }

            folderCollection.Context.Load(folderCollection);
            folderCollection.Context.ExecuteQueryRetry();
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
        /// <returns>The file contents as a string</returns>
        public static string GetFileAsString(this Web web, string serverRelativeUrl)
        {
            string returnString = string.Empty;

            var file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file);
            web.Context.ExecuteQueryRetry();
            ClientResult<Stream> stream = file.OpenBinaryStream();
            web.Context.ExecuteQueryRetry();

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
            context.ExecuteQueryRetry();

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
        /// <param name="comment">Comment recorded with the publish action</param>
        public static void PublishFile(this Web web, string serverRelativeUrl, string comment)
        {
            File file = null;
            file = web.GetFileByServerRelativeUrl(serverRelativeUrl);
            web.Context.Load(file, x => x.Exists, x => x.CheckOutType);
            web.Context.ExecuteQueryRetry();

            if (file.Exists)
            {
                file.Publish(comment);
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Gets a folder with a given name in a given <see cref="Microsoft.SharePoint.Client.Folder"/>
        /// </summary>
        /// <param name="folder"><see cref="Microsoft.SharePoint.Client.Folder"/> in which to search for</param>
        /// <param name="folderName">Name of the folder to search for</param>
        /// <returns>The found <see cref="Microsoft.SharePoint.Client.Folder"/> if available, null otherwise</returns>
        public static Folder ResolveSubFolder(this Folder folder, string folderName)
        {
            if (string.IsNullOrEmpty(folderName))
            {
                throw new ArgumentNullException("folderName");
            }

            folder.Context.Load(folder);
            folder.Context.Load(folder.Folders);
            folder.Context.ExecuteQueryRetry();

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
            clientContext.ExecuteQueryRetry();

            ClientResult<Stream> stream = file.OpenBinaryStream();
            clientContext.ExecuteQueryRetry();

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

        /// <summary>
        /// Uploads a file to the specified folder.
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="localFilePath">Location of the file to be uploaded.</param>
        /// <param name="overwriteIfExists">true (default) to overwite existing files</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static File UploadFile(this Folder folder, string fileName, string localFilePath, bool overwriteIfExists)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (localFilePath == null)
            {
                throw new ArgumentNullException("localFilePath");
            }

            if (!System.IO.File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Local file was not found.", localFilePath);
            }

            using (var stream = System.IO.File.OpenRead(localFilePath))
                return folder.UploadFile(fileName, stream, overwriteIfExists);
        }

        /// <summary>
        /// Uploads a file to the specified folder.
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="filePath">Location of the file to be uploaded.</param>
        /// <param name="overwriteIfExists">true (default) to overwite existing files</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static File UploadFile(this Folder folder, string fileName, Stream stream, bool overwriteIfExists)
        {
           if (fileName == null) 
            {
                throw new ArgumentNullException("fileName"); 
            }

            if (stream == null) 
            {
                throw new ArgumentNullException("localStream"); 
            }

            if (string.IsNullOrWhiteSpace(fileName)) 
            {
                throw new ArgumentException("Destination file name is required.", "fileName"); 
            }

            if (Regex.IsMatch(fileName, REGEX_INVALID_FILE_NAME_CHARS))
            {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            // Create the file
            var newFileInfo = new FileCreationInformation()
            {
                ContentStream = stream,
                Url = fileName,
                Overwrite = overwriteIfExists
            };

            LoggingUtility.Internal.TraceVerbose("Creating file info with Url '{0}'", newFileInfo.Url);
            var file = folder.Files.Add(newFileInfo);
            folder.Context.Load(file);
            folder.Context.ExecuteQueryRetry();

            return file;
        }

        /// <summary>
        /// Uploads a file to the specified folder by saving the binary directly (via webdav).
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="localFilePath">Location of the file to be uploaded.</param>
        /// <param name="overwriteIfExists">true (default) to overwite existing files</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static File UploadFileWebDav(this Folder folder, string fileName, string localFilePath, bool overwriteIfExists)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (localFilePath == null)
            {
                throw new ArgumentNullException("localFilePath");
            }

            if (!System.IO.File.Exists(localFilePath))
            {
                throw new FileNotFoundException("Local file was not found.", localFilePath);
            }

            using (var stream = System.IO.File.OpenRead(localFilePath))
                return folder.UploadFileWebDav(fileName, stream, overwriteIfExists);
        }

        /// <summary>
        /// Uploads a file to the specified folder by saving the binary directly (via webdav).
        /// Note: this method does not work using app only token.
        /// </summary>
        /// <param name="folder">Folder to upload file to.</param>
        /// <param name="filePath">Location of the file to be uploaded.</param>
        /// <param name="overwriteIfExists">true (default) to overwite existing files</param>
        /// <returns>The uploaded File, so that additional operations (such as setting properties) can be done.</returns>
        public static File UploadFileWebDav(this Folder folder, string fileName, Stream stream, bool overwriteIfExists)
        {
            if (fileName == null) 
            {
                throw new ArgumentNullException("fileName"); 
            }

            if (stream == null) 
            {
                throw new ArgumentNullException("localStream"); 
            }

            if (string.IsNullOrWhiteSpace(fileName)) 
            {
                throw new ArgumentException("Destination file name is required.", "fileName"); 
            }

            if (Regex.IsMatch(fileName, REGEX_INVALID_FILE_NAME_CHARS))
            {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            var serverRelativeUrl = UrlUtility.Combine(folder.ServerRelativeUrl, fileName);

            // Create uploadContext to get a proper ClientContext instead of a ClientRuntimeContext
            using (var uploadContext = folder.Context.Clone(folder.Context.Url))
            {
                LoggingUtility.Internal.TraceVerbose("Save binary direct (via webdav) to '{0}'", serverRelativeUrl);
                File.SaveBinaryDirect(uploadContext, serverRelativeUrl, stream, overwriteIfExists);
                uploadContext.ExecuteQueryRetry();
            }

            var file = folder.Files.GetByUrl(serverRelativeUrl);
            folder.Context.Load(file);
            folder.Context.ExecuteQueryRetry();

            return file;
        }

        /// <summary>
        /// Gets a file in a document library.
        /// </summary>
        /// <param name="folder">Folder containing the target file.</param>
        /// <param name="fileName">File name.</param>
        /// <returns>The target file if found, null if no file is found.</returns>
        public static File GetFile(this Folder folder, string fileName)
        {
            if (folder == null)
            {
                throw new ArgumentNullException("folder");
            }

            if (string.IsNullOrEmpty(fileName))
            {
                throw new ArgumentNullException("fileName");
            }

            try
            {
                if (!folder.IsPropertyAvailable("ServerRelativeUrl"))
                {
                    folder.Context.Load(folder, w => w.ServerRelativeUrl);
                    folder.Context.ExecuteQueryRetry();
                }

                var fileServerRelativeUrl = UrlUtility.Combine(folder.ServerRelativeUrl, fileName);                
                var web = folder.ListItemAllFields.ParentList.ParentWeb;                

                var file = web.GetFileByServerRelativeUrl(fileServerRelativeUrl);
                folder.Context.Load(file);
                folder.Context.ExecuteQueryRetry();
                return file;
            }
            catch (Exception ex)
            {
                if (ex.Message == "File Not Found.")
                {
                    return null;
                }
                throw;
            }
        }

        /// <summary>
        /// Used to compare the server file to the local file.
        /// This enables users with faster download speeds but slow upload speeds to evaluate if the server file should be overwritten.
        /// </summary>
        /// <param name="serverFile">File located on the server.</param>
        /// <param name="localFile">File to validate against.</param>
        public static bool VerifyIfUploadRequired(this File serverFile, string localFile)
        {
            if (localFile == null)
            {
                throw new ArgumentNullException("localFile");
            }

            if (!System.IO.File.Exists(localFile))
            {
                throw new FileNotFoundException("Local file was not found.", localFile);
            }

            using (var file = System.IO.File.OpenRead(localFile))
                return serverFile.VerifyIfUploadRequired(file);
        }

        /// <summary>
        /// Used to compare the server file to the local file.
        /// This enables users with faster download speeds but slow upload speeds to evaluate if the server file should be overwritten.
        /// </summary>
        /// <param name="serverFile">File located on the server.</param>
        /// <param name="localStream">Stream to validate against.</param>
        /// <returns></returns>
        public static bool VerifyIfUploadRequired(this File serverFile, Stream localStream)
        {
            if (serverFile == null)
            {
                throw new ArgumentNullException("serverFile");
            }

            if (localStream == null)
            {
                throw new ArgumentNullException("localStream");
            }

            byte[] serverHash = null;
            var streamResult = serverFile.OpenBinaryStream();
            serverFile.Context.ExecuteQueryRetry();

            // Hash contents
            HashAlgorithm ha = HashAlgorithm.Create();
            using (var serverStream = streamResult.Value)
            {
                serverHash = ha.ComputeHash(serverStream);
                //Console.WriteLine("Server hash: {0}", BitConverter.ToString(serverHash));
            }

            // Check hash (& rewind)
            byte[] localHash;
            localHash = ha.ComputeHash(localStream);
            localStream.Position = 0;
            //Console.WriteLine("Local hash: {0}", BitConverter.ToString(localHash));

            // Compare hash
            var contentsMatch = true;
            for (var index = 0; index < serverHash.Length; index++)
            {
                if (serverHash[index] != localHash[index])
                {
                    //Console.WriteLine("Hash does not match");
                    contentsMatch = false;
                    break;
                }
            }

            localStream.Position = 0;
            return !contentsMatch;
        }

        /// <summary>
        /// Sets file properties using a dictionary.
        /// </summary>
        /// <param name="file">Target file object.</param>
        /// <param name="properties">Dictionary of properties to set.</param>
        /// <param name="checkoutIfRequired">Check out the file if necessary to set properties.</param>
        public static void SetFileProperties(this File file, IDictionary<string, string> properties, bool checkoutIfRequired = true)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            if (properties == null)
            {
                throw new ArgumentNullException("properties");
            }

            var changedProperties = new Dictionary<string, string>();
            var changedPropertiesString = new StringBuilder();
            var context = file.Context;

            if (properties != null && properties.Count > 0)
            {
                // If this throws ServerException (does not belong to list), then shouldn't be trying to set properties)
                context.Load(file.ListItemAllFields);
                context.Load(file.ListItemAllFields.FieldValuesAsText);
                context.ExecuteQueryRetry();

                // Loop through and detect changes first, then, check out if required and apply
                foreach (var kvp in properties)
                {
                    var propertyName = kvp.Key;
                    var propertyValue = kvp.Value;

                    var fieldValues = file.ListItemAllFields.FieldValues;
                    var currentValue = string.Empty;
                    if (file.ListItemAllFields.FieldValues.ContainsKey(propertyName))
                    {
                        currentValue = file.ListItemAllFields.FieldValuesAsText[propertyName];
                    }

                    //LoggingUtility.Internal.TraceVerbose("*** Comparing property '{0}' to current '{1}', new '{2}'", propertyName, currentValue, propertyValue);
                    switch (propertyName.ToUpperInvariant())
                    {
                        case "CONTENTTYPE":
                            {
                                // TODO: Add support for named ContentType (need to lookup ID and check if it needs changing)
                                throw new NotSupportedException("ContentType property not yet supported; use ContentTypeId instead.");
                                //break;
                            }
                        case "CONTENTTYPEID":
                            {
                                var currentBase = currentValue.Substring(0, currentValue.Length - 34);
                                var sameValue = (currentBase == propertyValue);
                                if (!sameValue && propertyValue.Length >= 32 + 6 && propertyValue.Substring(propertyValue.Length - 34, 2) == "00")
                                {
                                    var propertyBase = propertyValue.Substring(0, propertyValue.Length - 34);
                                    sameValue = (currentBase == propertyBase);
                                }

                                if (!sameValue)
                                {
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                        case "PUBLISHINGASSOCIATEDCONTENTTYPE":
                            {
                                var testValue = ";#" + currentValue.Replace(", ", ";#") + ";#";
                                if (testValue != propertyValue)
                                {
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                        default:
                            {
                                if (currentValue != propertyValue)
                                {
                                    //Console.WriteLine("Setting property '{0}' to '{1}'", propertyName, propertyValue);
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                    }
                }

                if (changedProperties.Count > 0)
                {
                    LoggingUtility.Internal.TraceInformation((int)EventId.UpdateFileProperties, CoreResources.FileFolderExtensions_UpdateFile0Properties1, file.Name, changedPropertiesString);
                    var checkOutRequired = false;

                    var parentList = file.ListItemAllFields.ParentList;
                    context.Load(parentList, l => l.ForceCheckout);
                    try
                    {
                        context.ExecuteQueryRetry();
                        checkOutRequired = parentList.ForceCheckout;
                    }
                    catch (ServerException ex)
                    {
                        if (ex.Message != "The object specified does not belong to a list.")
                        {
                            throw;
                        }
                    }

                    if (checkOutRequired && file.CheckOutType == CheckOutType.None)
                    {
                        LoggingUtility.Internal.TraceVerbose("Checking out file '{0}'", file.Name);
                        file.CheckOut();
                        context.ExecuteQueryRetry();
                    }

                    LoggingUtility.Internal.TraceVerbose("Set properties: {0}", file.Name);
                    foreach (var kvp in changedProperties)
                    {
                        var propertyName = kvp.Key;
                        var propertyValue = kvp.Value;

                        LoggingUtility.Internal.TraceVerbose(" {0}={1}", propertyName, propertyValue);
                        file.ListItemAllFields[propertyName] = propertyValue;
                    }
                    file.ListItemAllFields.Update();
                    context.ExecuteQueryRetry();
                }
            }
        }

        /// <summary>
        /// Publishes a file based on the type of versioning required on the parent library.
        /// </summary>
        /// <param name="file">Target file to publish.</param>
        /// <param name="level">Target publish direction (Draft and Published only apply, Checkout is ignored).</param>
        public static void PublishFileToLevel(this File file, FileLevel level)
        {
            if (file == null)
            {
                throw new ArgumentNullException("file");
            }

            var publishingRequired = false;
            var approvalRequired = false;
            var checkOutRequired = false;

            if (level == FileLevel.Draft || level == FileLevel.Published)
            {
                var context = file.Context;
                var parentList = file.ListItemAllFields.ParentList;
                context.Load(parentList,
                            l => l.EnableMinorVersions,
                            l => l.EnableModeration,
                            l => l.ForceCheckout);

                checkOutRequired = parentList.ForceCheckout;

                try
                {
                    context.ExecuteQueryRetry();
                    publishingRequired = parentList.EnableMinorVersions; // minor versions implies that the file must be published
                    approvalRequired = parentList.EnableModeration;
                }
                catch (ServerException ex)
                {
                    if (ex.Message != "The object specified does not belong to a list.")
                    {
                        throw;
                    }
                }

                if (file.CheckOutType != CheckOutType.None || checkOutRequired)
                {
                    LoggingUtility.Internal.TraceVerbose("Checking in file '{0}'", file.Name);
                    file.CheckIn("Checked in by provisioning", publishingRequired ? CheckinType.MinorCheckIn : CheckinType.MajorCheckIn);
                    context.ExecuteQueryRetry();
                }

                if (level == FileLevel.Published)
                {
                    if (publishingRequired)
                    {
                        LoggingUtility.Internal.TraceVerbose("Publishing file '{0}'", file.Name);
                        file.Publish("Published by provisioning");
                        context.ExecuteQueryRetry();
                    }

                    if (approvalRequired)
                    {
                        LoggingUtility.Internal.TraceVerbose("Approving file '{0}'", file.Name);
                        file.Approve("Approved by provisioning");
                        context.ExecuteQueryRetry();
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
