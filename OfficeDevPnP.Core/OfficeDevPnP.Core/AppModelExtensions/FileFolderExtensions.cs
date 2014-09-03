using OfficeDevPnP.Core.Utilities;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
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
        /// Uploads a file to a server relative url
        /// </summary>
        /// <param name="web">The web to process</param>
        /// <param name="filePath">Full path to the file like c:\temp\fuu.txt</param>
        /// <param name="serverRelativeUrl">Full server relative destination url of the file on the server</param>
        /// <param name="useWebDav">Use webdav uploads, better suitable for larger files.</param>
        public static void UploadFileToServerRelativeUrl(this Web web, string filePath, string serverRelativeUrl, bool useWebDav = false)
        {
            if(!serverRelativeUrl.ToLower().EndsWith(System.IO.Path.GetFileName(filePath).ToLower()))
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
        /// Ensure that the folder structure is created. This also ensures hierarchy of folders
        /// </summary>
        /// <param name="web">Web to be processed - can be root web or sub site</param>
        /// <param name="parentFolder">Parent folder</param>
        /// <param name="folderPath">Folder path</param>
        /// <returns>The folder structure</returns>
        public static Folder EnsureFolder(this Web web, Folder parentFolder, string folderPath)
        {
            // Split up the incoming path so we have the first element as the a new sub-folder name 
            // and add it to ParentFolder folders collection
            string[] pathElements = folderPath.Split(new char[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            string head = pathElements[0];

            Folder newFolder = parentFolder.Folders.Add(head);
            web.Context.Load(newFolder);
            web.Context.ExecuteQuery();

            // If we have subfolders to create then the length of PathElements will be greater than 1
            if (pathElements.Length > 1)
            {
                // If we have more nested folders to create then reassemble the folder path using what we have left i.e. the tail
                string Tail = string.Empty;

                for (int i = 1; i < pathElements.Length; i++)
                {
                    Tail = Tail + "/" + pathElements[i];
                }

                // Then make a recursive call to create the next subfolder
                return web.EnsureFolder(newFolder, Tail);
            }
            else
            {
                // This ensures that the folder at the end of the chain gets returned
                return newFolder;
            }
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

        [Obsolete("Use FolderExists() instead, which works for both web sites and subfolders.")]
        public static bool SubFolderExists(this Folder folder, string folderName)
        {
            return folder.FolderExists(folderName);
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

        private static string WildcardToRegex(string pattern)
        {
            return "^" + Regex.Escape(pattern).
                               Replace(@"\*", ".*").
                               Replace(@"\?", ".") + "$";
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

    }
}
