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
                    clientContext.ExecuteQueryRetry();
                    Microsoft.SharePoint.Client.File.SaveBinaryDirect(clientContext, serverRelativeUrl, fs, true);
                }
            }
            else
            {
                var files = web.RootFolder.Files;
                clientContext.Load(files);

                clientContext.ExecuteQueryRetry();

                if (files != null)
                {
                    using (FileStream stream = new FileStream(filePath, FileMode.Open))
                    {
                        FileCreationInformation createInfo = new FileCreationInformation();
                        createInfo.ContentStream = stream;

                        createInfo.Overwrite = true;
                        createInfo.Url = serverRelativeUrl;
                        files.Add(createInfo);
                        clientContext.ExecuteQueryRetry();
                    }
                }
            }
        }

        [Obsolete("Prefer folder.UploadFile() or folder.UploadFileWebDav().")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static File UploadFile(this Folder folder, string filePath, ContentTypeId contentTypeId, bool overwriteIfExists = true, bool useWebDav = true) {
            if (filePath == null) { throw new ArgumentNullException("filePath"); }
            if (string.IsNullOrWhiteSpace(filePath)) { throw new ArgumentException("File path is required.", "filePath"); }

            var fileName = System.IO.Path.GetFileName(filePath);
            using (var fs = new System.IO.FileStream(filePath, System.IO.FileMode.Open)) {
                var additionalProperties = new Dictionary<string, string>();
                if (contentTypeId != null) {
                    additionalProperties["ContentTypeId"] = contentTypeId.StringValue;
                }
                return UploadFile(folder, fileName, fs, additionalProperties: additionalProperties, replaceContent: overwriteIfExists, checkHashBeforeUpload: true, level: FileLevel.Published, useWebDav: useWebDav);
            }
        }

        [Obsolete("Prefer folder.UploadFile() or folder.UploadFileWebDav().")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static File UploadFile(this Folder folder, string fileName, Stream stream, ContentTypeId contentTypeId, bool overwriteIfExists = true, bool useWebDav = true) {
            var additionalProperties = new Dictionary<string, string>();
            if (contentTypeId != null) {
                additionalProperties["ContentTypeId"] = contentTypeId.StringValue;
            }
            return UploadFile(folder, fileName, stream, additionalProperties: additionalProperties, replaceContent: overwriteIfExists, checkHashBeforeUpload: true, level: FileLevel.Published, useWebDav: useWebDav);
        }

        [Obsolete("Prefer folder.UploadFile() or folder.UploadFileWebDav(). Also can use file.SetFileProperties() and file.VerifyIfUploadRequired().")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static File UploadFile(this Folder folder, string localFilePath, IDictionary<string, string> additionalProperties = null, bool replaceContent = true, bool checkHashBeforeUpload = true, FileLevel level = FileLevel.Published, bool useWebDav = true) {
            if (localFilePath == null) { throw new ArgumentNullException("localFilePath"); }
            if (string.IsNullOrWhiteSpace(localFilePath)) { throw new ArgumentException("Source file path is required.", "localFilePath"); }

            var fileName = System.IO.Path.GetFileName(localFilePath);
            using (var localStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open)) {
                return UploadFile(folder, fileName, localStream, additionalProperties, replaceContent, checkHashBeforeUpload, level, useWebDav);
            }
        }

        [Obsolete("Prefer folder.UploadFile() or folder.UploadFileWebDav(). Also can use file.SetFileProperties() and file.VerifyIfUploadRequired().")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static File UploadFile(this Folder folder, string fileName, string localFilePath, IDictionary<string, string> additionalProperties = null, bool replaceContent = true, bool checkHashBeforeUpload = true, FileLevel level = FileLevel.Published, bool useWebDav = true) {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentException("Destination file name is required.", "fileName"); }
            if (localFilePath == null) { throw new ArgumentNullException("localFilePath"); }
            if (string.IsNullOrWhiteSpace(localFilePath)) { throw new ArgumentException("Source file path is required.", "localFilePath"); }

            //Console.WriteLine("Provisioning file '{0}' to '{1}'", localFilePath, fileName);

            using (var localStream = new System.IO.FileStream(localFilePath, System.IO.FileMode.Open)) {
                return UploadFile(folder, fileName, localStream, additionalProperties, replaceContent, checkHashBeforeUpload, level, useWebDav);
            }
        }

        [Obsolete("Prefer folder.UploadFile() or folder.UploadFileWebDav(). Also can use file.SetFileProperties() and file.VerifyIfUploadRequired().")]
        [EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static File UploadFile(this Folder folder, string fileName, System.IO.Stream localStream, IDictionary<string, string> additionalProperties = null, bool replaceContent = true, bool checkHashBeforeUpload = true, FileLevel level = FileLevel.Published, bool useWebDav = true) {
            if (fileName == null) { throw new ArgumentNullException("fileName"); }
            if (localStream == null) { throw new ArgumentNullException("localStream"); }
            if (string.IsNullOrWhiteSpace(fileName)) { throw new ArgumentException("Destination file name is required.", "fileName"); }
            // TODO: Check for any other illegal characters in SharePoint
            if (fileName.Contains('/') || fileName.Contains('\\')) {
                throw new ArgumentException("The argument must be a single file name and cannot contain path characters.", "fileName");
            }

            // Check for existing file
            if (!folder.IsObjectPropertyInstantiated("ServerRelativeUrl")) {
                folder.Context.Load(folder, f => f.ServerRelativeUrl);
                folder.Context.ExecuteQueryRetry();
            }
            var serverRelativeUrl = UrlUtility.Combine(folder.ServerRelativeUrl, fileName);

            bool checkOutRequired = false;
            bool publishingRequired = false;
            bool approvalRequired = false;

            // Check for existing file
            var fileCollection = folder.Files;
            File existingFile = null;
            folder.Context.Load(fileCollection);
            folder.Context.ExecuteQueryRetry();
            foreach (var checkFile in fileCollection) {
                if (string.Equals(checkFile.Name, fileName, StringComparison.InvariantCultureIgnoreCase)) {
                    existingFile = checkFile;
                    break;
                }
            }

            // Determine if upload required
            bool uploadRequired = false;
            byte[] serverHash = null;
            if (existingFile != null) {
                if (replaceContent) {
                    if (checkHashBeforeUpload) {
                        var streamResult = existingFile.OpenBinaryStream();
                        folder.Context.ExecuteQueryRetry();
                        // Hash contents
                        HashAlgorithm ha = HashAlgorithm.Create();
                        using (var serverStream = streamResult.Value) {
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
                        for (var index = 0; index < serverHash.Length; index++) {
                            if (serverHash[index] != localHash[index]) {
                                //Console.WriteLine("Hash does not match");
                                contentsMatch = false;
                                break;
                            }
                        }
                        uploadRequired = !contentsMatch;
                    }
                    else {
                        //Console.WriteLine("Not checking if existing file is the same; force upload");
                        uploadRequired = true;
                    }
                }
                else {
                    throw new Exception("File already exists, replace contents needs to be specified.");
                }
            }
            else {
                uploadRequired = true;
            }

            File file = null;
            // If different, upload file
            if (uploadRequired) {
                LoggingUtility.Internal.TraceInformation((int)EventId.UploadFile, CoreResources.FileFolderExtensions_UploadFile0ToFolder1, fileName, folder.ServerRelativeUrl);

                if (existingFile != null) {
                    // Existing file (upload required) -- determine if checkout required
                    var parentList = existingFile.ListItemAllFields.ParentList;
                    folder.Context.Load(parentList, l => l.ForceCheckout);
                    try {
                        folder.Context.ExecuteQueryRetry();
                        checkOutRequired = parentList.ForceCheckout;
                    }
                    catch (ServerException ex) {
                        if (ex.Message != "The object specified does not belong to a list.") {
                            throw;
                        }
                    }
                    //LoggingUtility.Internal.TraceVerbose("*** ForceCheckout {0}", checkOutRequired);

                    if (checkOutRequired && existingFile.CheckOutType == CheckOutType.None) {
                        LoggingUtility.Internal.TraceVerbose("Checking out file '{0}'", fileName);
                        existingFile.CheckOut();
                        folder.Context.ExecuteQueryRetry();
                    }
                }

                if (useWebDav) {
                    using (var uploadContext = new ClientContext(folder.Context.Url) { Credentials = folder.Context.Credentials }) {
                        LoggingUtility.Internal.TraceVerbose("Save binary direct (via webdav) to '{0}'", serverRelativeUrl);
                        File.SaveBinaryDirect(uploadContext, serverRelativeUrl, localStream, true);
                        uploadContext.ExecuteQueryRetry();
                    }
                    file = folder.Files.GetByUrl(serverRelativeUrl);
                }
                else {
                    FileCreationInformation fileCreation = new FileCreationInformation();
                    fileCreation.ContentStream = localStream;
                    fileCreation.Url = fileName;
                    fileCreation.Overwrite = true;
                    LoggingUtility.Internal.TraceVerbose("Creating file info with Url '{0}'", fileCreation.Url);
                    file = folder.Files.Add(fileCreation);
                    folder.Context.ExecuteQueryRetry();
                }
            }
            else {
                //LoggingUtility.Internal.TraceVerbose("Not uploading; existing file '{0}' in folder '{1}' is identical (hash {2})", fileName, folder.ServerRelativeUrl, BitConverter.ToString(serverHash));
                LoggingUtility.Internal.TraceVerbose("Not uploading; existing file '{0}' is identical", fileName);
                file = existingFile;
            }

            folder.Context.Load(file);
            folder.Context.ExecuteQueryRetry();

            // Set file properties (child elements <Property>)
            var changedProperties = new Dictionary<string, string>();
            var changedPropertiesString = new StringBuilder();
            var propertyChanged = false;
            if (additionalProperties != null && additionalProperties.Count > 0) {
                // If this throws ServerException (does not belong to list), then shouldn't be trying to set properties)
                folder.Context.Load(file.ListItemAllFields);
                folder.Context.Load(file.ListItemAllFields.FieldValuesAsText);
                folder.Context.ExecuteQueryRetry();

                // Loop through and detect changes first, then, check out if required and apply
                foreach (var kvp in additionalProperties) {
                    var propertyName = kvp.Key;
                    var propertyValue = kvp.Value;

                    var fieldValues = file.ListItemAllFields.FieldValues;
                    var currentValue = "";
                    if (file.ListItemAllFields.FieldValues.ContainsKey(propertyName)) {
                        currentValue = file.ListItemAllFields.FieldValuesAsText[propertyName];
                    }
                    //LoggingUtility.Internal.TraceVerbose("*** Comparing property '{0}' to current '{1}', new '{2}'", propertyName, currentValue, propertyValue);
                    switch (propertyName.ToUpperInvariant()) {
                        case "CONTENTTYPE": {
                                // TODO: Add support for named ContentType (need to lookup ID and check if it needs changing)
                                throw new NotSupportedException("ContentType property not yet supported; use ContentTypeId instead.");
                                //break;
                            }
                        case "CONTENTTYPEID": {
                                var currentBase = currentValue.Substring(0, currentValue.Length - 34);
                                var sameValue = (currentBase == propertyValue);
                                if (!sameValue && propertyValue.Length >= 32 + 6 && propertyValue.Substring(propertyValue.Length - 34, 2) == "00") {
                                    var propertyBase = propertyValue.Substring(0, propertyValue.Length - 34);
                                    sameValue = (currentBase == propertyBase);
                                }
                                if (!sameValue) {
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                        case "PUBLISHINGASSOCIATEDCONTENTTYPE": {
                                var testValue = ";#" + currentValue.Replace(", ", ";#") + ";#";
                                if (testValue != propertyValue) {
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                        default: {
                                if (currentValue != propertyValue) {
                                    //Console.WriteLine("Setting property '{0}' to '{1}'", propertyName, propertyValue);
                                    changedProperties[propertyName] = propertyValue;
                                    changedPropertiesString.AppendFormat("{0}='{1}'; ", propertyName, propertyValue);
                                }
                                break;
                            }
                    }
                }

                if (changedProperties.Count > 0) {
                    if (!uploadRequired) {
                        LoggingUtility.Internal.TraceInformation((int)EventId.UpdateFileProperties, CoreResources.FileFolderExtensions_UpdateFile0Properties1, fileName, changedPropertiesString);
                        if (existingFile != null) {
                            // Existing file (no upload required, but properties were changed) -- determine if checkout required
                            var parentList = file.ListItemAllFields.ParentList;
                            folder.Context.Load(parentList, l => l.ForceCheckout);
                            try {
                                folder.Context.ExecuteQueryRetry();
                                checkOutRequired = parentList.ForceCheckout;
                            }
                            catch (ServerException ex) {
                                if (ex.Message != "The object specified does not belong to a list.") {
                                    throw;
                                }
                            }
                            //LoggingUtility.Internal.TraceVerbose("*** ForceCheckout2 {0}", checkOutRequired, approvalRequired);

                            if (checkOutRequired && file.CheckOutType == CheckOutType.None) {
                                LoggingUtility.Internal.TraceVerbose("Checking out file '{0}'", fileName);
                                file.CheckOut();
                                folder.Context.ExecuteQueryRetry();
                            }
                        }
                    }
                    else {
                        LoggingUtility.Internal.TraceVerbose("Updating properties of file '{0}' after upload: {1}", fileName, changedPropertiesString);
                    }

                    foreach (var kvp in changedProperties) {
                        var propertyName = kvp.Key;
                        var propertyValue = kvp.Value;

                        file.ListItemAllFields[propertyName] = propertyValue;
                    }
                    file.ListItemAllFields.Update();
                    folder.Context.ExecuteQueryRetry();
                    propertyChanged = true;
                }
            }

            //LoggingUtility.Internal.TraceVerbose("*** Up {0}, Prop {1}, COT {2}, level", uploadRequired, propertyChanged, file.CheckOutType, level);

            if (uploadRequired || propertyChanged && (level == FileLevel.Draft || level == FileLevel.Published)) {
                var parentList2 = file.ListItemAllFields.ParentList;
                folder.Context.Load(parentList2, l => l.EnableMinorVersions, l => l.EnableModeration);
                try {
                    folder.Context.ExecuteQueryRetry();
                    publishingRequired = parentList2.EnableMinorVersions;
                    approvalRequired = parentList2.EnableModeration;
                }
                catch (ServerException ex) {
                    if (ex.Message != "The object specified does not belong to a list.") {
                        throw;
                    }
                }
                //LoggingUtility.Internal.TraceVerbose("*** EnableMinorVerions {0}. EnableModeration {1}", publishingRequired, approvalRequired);

                if (file.CheckOutType != CheckOutType.None || checkOutRequired) {
                    LoggingUtility.Internal.TraceVerbose("Checking in file '{0}'", fileName);
                    file.CheckIn("Checked in by provisioning", publishingRequired ? CheckinType.MinorCheckIn : CheckinType.MajorCheckIn);
                    folder.Context.ExecuteQueryRetry();
                }

                if (level == FileLevel.Published) {
                    if (publishingRequired) {
                        LoggingUtility.Internal.TraceVerbose("Publishing file '{0}'", fileName);
                        file.Publish("Published by provisioning");
                        folder.Context.ExecuteQueryRetry();
                    }
                    if (approvalRequired) {
                        LoggingUtility.Internal.TraceVerbose("Approving file '{0}'", fileName);
                        file.Approve("Approved by provisioning");
                        folder.Context.ExecuteQueryRetry();
                    }
                }
            }

            return file;
        }
    }
}
