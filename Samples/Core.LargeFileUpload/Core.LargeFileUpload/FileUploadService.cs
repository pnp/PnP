using Microsoft.SharePoint.Client;
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Contoso.Core.LargeFileUpload
{
    /// <summary>
    /// Encapsulate file upload services for demo purposes
    /// </summary>
    public class FileUploadService
    {
        private long fileoffset;

        /// <summary>
        /// Uploads a large file slice per slice
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="libraryName"></param>
        /// <param name="fileName"></param>
        /// <param name="fileChunkSizeInMB"></param>
        public void UploadFileSlicePerSlice(ClientContext ctx, string libraryName, string fileName, int fileChunkSizeInMB = 3)
        {

            // Each sliced upload requires a unique id
            Guid uploadId = Guid.NewGuid();

            // Get the name of the file
            string uniqueFileName = Path.GetFileName(fileName);

            // Set fileoffset to 0
            fileoffset = 0;

            // Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, ctx.Web, libraryName))
            {
                CreateLibrary(ctx, ctx.Web, libraryName);
            }
            // Get to folder to upload into 
            List docs = ctx.Web.Lists.GetByTitle(libraryName);
            ctx.Load(docs, l => l.RootFolder);
            // Get the information about the folder that will hold the file
            ctx.Load(docs.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            // **Important**: below sample code is for educational purposes. In a 
            // real life implementation one would read the "slices" of slice 
            // per slice from the filesystem instead of reading and dividing
            // the whole file in memory

            // Read the file data in bytes array
            byte[] fileData = System.IO.File.ReadAllBytes(fileName);

            // Prepare for splitting the fileData in chunks
            List<byte[]> sliceData = new List<byte[]>();
            int lengthToSplit = fileChunkSizeInMB * 1024 * 1024;
            int arrayLength = fileData.Length;
            int byteCount = 0;

            // Split the binary data in chunks
            for (int i = 0; i < arrayLength; i = i + lengthToSplit)
            {
                if (byteCount + lengthToSplit > arrayLength)
                {
                    lengthToSplit = arrayLength - byteCount;
                }

                byte[] val = new byte[lengthToSplit];

                if (arrayLength < i + lengthToSplit)
                {
                    lengthToSplit = arrayLength - i;
                }

                Array.Copy(fileData, i, val, 0, lengthToSplit);
                sliceData.Add(val);
                byteCount = byteCount + lengthToSplit;
            }

            // How many slices do we have
            int sliceCount = sliceData.Count;

            // **Important**: below sample code is for educational purposes. In a 
            // real life implementation one would store the state after each successful
            // slice upload so that the upload can be restarted from the last slice that 
            // was successfully uploaded

            // upload slice per slice. They'll need to be uploaded in the correct order
            int sliceNumber = 0;
            foreach (byte[] slice in sliceData)
            {
                UploadFileSlice(ctx, uploadId, slice, docs.RootFolder, uniqueFileName, sliceNumber, sliceCount);
                sliceNumber++;
            }
        }

        private void UploadFileSlice(ClientContext cc, Guid uploadId, Byte[] sliceContent, Folder folder, string uniqueFilename, int sliceNumber, int totalSlices)
        {
            // Is this the last slice
            bool isFinalSlice = sliceNumber == totalSlices - 1;

            Microsoft.SharePoint.Client.File uploadFile;
            ClientResult<long> bytesUploaded = null;

            if (sliceNumber == 0)
            {
                // First slice
                using (MemoryStream contentStream = new MemoryStream())
                {
                    // Add an empty file.
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = contentStream;
                    fileInfo.Url = uniqueFilename;
                    fileInfo.Overwrite = true;

                    uploadFile = folder.Files.Add(fileInfo);

                    // Start upload by uploading the first slice. 
                    using (MemoryStream s = new MemoryStream(sliceContent))
                    {
                        // Call the start upload method on the first slice
                        bytesUploaded = uploadFile.StartUpload(uploadId, s);
                        cc.ExecuteQuery();
                        // fileoffset is the pointer where the next slice will be added
                        fileoffset = bytesUploaded.Value;
                    }
                }
            }
            else
            {
                // Get a reference to our file
                uploadFile = cc.Web.GetFileByServerRelativeUrl(folder.ServerRelativeUrl + System.IO.Path.AltDirectorySeparatorChar + uniqueFilename);
                using (MemoryStream s = new MemoryStream(sliceContent))
                {
                    if (isFinalSlice)
                    {
                        // End sliced upload by calling FinishUpload
                        uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, s);
                        cc.ExecuteQuery();
                    }
                    else
                    {
                        // Continue sliced upload
                        bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, s);
                        cc.ExecuteQuery();
                        // update fileoffset for the next slice
                        fileoffset = bytesUploaded.Value;
                    }
                }
            }
        }


        /// <summary>
        /// This has limitation of roughly 2 MB as the file size due the way information is sent to the server
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="libraryName"></param>
        /// <param name="filePath"></param>
        /// <param name="createLibraryIfNotExists"></param>
        public void UploadDocumentContent(ClientContext ctx, string libraryName, string filePath)
        {
            Web web = ctx.Web;

            // Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, web, libraryName))
            {
                CreateLibrary(ctx, web, libraryName);
            }

            FileCreationInformation newFile = new FileCreationInformation();
            newFile.Content = System.IO.File.ReadAllBytes(filePath);
            newFile.Url = System.IO.Path.GetFileName(filePath);

            // Get instances to the given library
            List docs = web.Lists.GetByTitle(libraryName);
            // Add file to the library
            Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(newFile);
            ctx.Load(uploadFile);
            ctx.ExecuteQuery();
        }

        /// <summary>
        /// Valid approach for large files
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="libraryName"></param>
        /// <param name="filePath"></param>
        public void SaveBinaryDirect(ClientContext ctx, string libraryName, string filePath)
        {
            Web web = ctx.Web;
            // Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, web, libraryName))
            {
                CreateLibrary(ctx, web, libraryName);
            }

            List docs = ctx.Web.Lists.GetByTitle(libraryName);
            ctx.Load(docs, l => l.RootFolder);
            // Get the information about the folder that will hold the file
            ctx.Load(docs.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                Microsoft.SharePoint.Client.File.SaveBinaryDirect(ctx, string.Format("{0}/{1}", docs.RootFolder.ServerRelativeUrl, System.IO.Path.GetFileName(filePath)), fs, true);
            }

        }

        /// <summary>
        /// Another valid approach for large files
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="library"></param>
        /// <param name="filePath"></param>
        public void UploadDocumentContentStream(ClientContext ctx, string libraryName, string filePath)
        {

            Web web = ctx.Web;
            // Ensure that target library exists, create if is missing
            if (!LibraryExists(ctx, web, libraryName))
            {
                CreateLibrary(ctx, web, libraryName);
            }

            using (FileStream fs = new FileStream(filePath, FileMode.Open))
            {
                FileCreationInformation flciNewFile = new FileCreationInformation();

                // This is the key difference for the first case - using ContentStream property
                flciNewFile.ContentStream = fs;
                flciNewFile.Url = System.IO.Path.GetFileName(filePath);
                flciNewFile.Overwrite = true;

                List docs = web.Lists.GetByTitle(libraryName);
                Microsoft.SharePoint.Client.File uploadFile = docs.RootFolder.Files.Add(flciNewFile);

                ctx.Load(uploadFile);
                ctx.ExecuteQuery();
            }
        }

        private bool LibraryExists(ClientContext ctx, Web web, string libraryName)
        {
            ListCollection lists = web.Lists;
            IEnumerable<List> results = ctx.LoadQuery<List>(lists.Where(list => list.Title == libraryName));
            ctx.ExecuteQuery();
            List existingList = results.FirstOrDefault();

            if (existingList != null)
            {
                return true;
            }

            return false;
        }


        private void CreateLibrary(ClientContext ctx, Web web, string libraryName)
        {
            // Create library to the web
            ListCreationInformation creationInfo = new ListCreationInformation();
            creationInfo.Title = libraryName;
            creationInfo.TemplateType = (int)ListTemplateType.DocumentLibrary;
            List list = web.Lists.Add(creationInfo);
            ctx.ExecuteQuery();
        }

    }
}
