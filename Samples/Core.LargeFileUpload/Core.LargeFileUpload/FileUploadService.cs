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

        /// <summary>
        /// Uploads a file using the sliced upload if the file is bigger then the block size, if not the ContentStream approach is used
        /// </summary>
        /// <param name="ctx"></param>
        /// <param name="libraryName"></param>
        /// <param name="fileName"></param>
        /// <param name="fileChunkSizeInMB"></param>
        /// <returns></returns>
        public Microsoft.SharePoint.Client.File UploadFileSlicePerSlice(ClientContext ctx, string libraryName, string fileName, int fileChunkSizeInMB = 3)
        {
            // Each sliced upload requires a unique id
            Guid uploadId = Guid.NewGuid();

            // Get the name of the file
            string uniqueFileName = Path.GetFileName(fileName);

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

            // File object 
            Microsoft.SharePoint.Client.File uploadFile;

            // Calculate block size in bytes
            int blockSize = fileChunkSizeInMB * 1024 * 1024;

            // Get the information about the folder that will hold the file
            ctx.Load(docs.RootFolder, f => f.ServerRelativeUrl);
            ctx.ExecuteQuery();


            // Get the size of the file
            long fileSize = new FileInfo(fileName).Length;

            if (fileSize <= blockSize)
            {
                // Use regular approach
                using (FileStream fs = new FileStream(fileName, FileMode.Open))
                {
                    FileCreationInformation fileInfo = new FileCreationInformation();
                    fileInfo.ContentStream = fs;
                    fileInfo.Url = uniqueFileName;
                    fileInfo.Overwrite = true;
                    uploadFile = docs.RootFolder.Files.Add(fileInfo);
                    ctx.Load(uploadFile);
                    ctx.ExecuteQuery();
                    // return the file object for the uploaded file
                    return uploadFile;
                }
            }
            else
            {
                // Use large file upload approach
                ClientResult<long> bytesUploaded = null;

                FileStream fs = null;
                try
                {
                    fs = System.IO.File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
                    using (BinaryReader br = new BinaryReader(fs))
                    {
                        byte[] buffer = new byte[blockSize];
                        Byte[] lastBuffer = null;
                        long fileoffset = 0;
                        long totalBytesRead = 0;
                        int bytesRead;
                        bool first = true;
                        bool last = false;

                        // Read data from filesystem in blocks 
                        while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                        {
                            totalBytesRead = totalBytesRead + bytesRead;

                            // We've reached the end of the file
                            if (totalBytesRead == fileSize)
                            {
                                last = true;
                                // Copy to a new buffer that has the correct size
                                lastBuffer = new byte[bytesRead];
                                Array.Copy(buffer, 0, lastBuffer, 0, bytesRead);
                            }

                            if (first)
                            {
                                using (MemoryStream contentStream = new MemoryStream())
                                {
                                    // Add an empty file.
                                    FileCreationInformation fileInfo = new FileCreationInformation();
                                    fileInfo.ContentStream = contentStream;
                                    fileInfo.Url = uniqueFileName;
                                    fileInfo.Overwrite = true;
                                    uploadFile = docs.RootFolder.Files.Add(fileInfo);

                                    // Start upload by uploading the first slice. 
                                    using (MemoryStream s = new MemoryStream(buffer))
                                    {
                                        // Call the start upload method on the first slice
                                        bytesUploaded = uploadFile.StartUpload(uploadId, s);
                                        ctx.ExecuteQuery();
                                        // fileoffset is the pointer where the next slice will be added
                                        fileoffset = bytesUploaded.Value;
                                    }

                                    // we can only start the upload once
                                    first = false;
                                }
                            }
                            else
                            {
                                // Get a reference to our file
                                uploadFile = ctx.Web.GetFileByServerRelativeUrl(docs.RootFolder.ServerRelativeUrl + System.IO.Path.AltDirectorySeparatorChar + uniqueFileName);

                                if (last)
                                {
                                    // Is this the last slice of data?
                                    using (MemoryStream s = new MemoryStream(lastBuffer))
                                    {
                                        // End sliced upload by calling FinishUpload
                                        uploadFile = uploadFile.FinishUpload(uploadId, fileoffset, s);
                                        ctx.ExecuteQuery();

                                        // return the file object for the uploaded file
                                        return uploadFile;
                                    }
                                }
                                else
                                {
                                    using (MemoryStream s = new MemoryStream(buffer))
                                    {
                                        // Continue sliced upload
                                        bytesUploaded = uploadFile.ContinueUpload(uploadId, fileoffset, s);
                                        ctx.ExecuteQuery();
                                        // update fileoffset for the next slice
                                        fileoffset = bytesUploaded.Value;
                                    }
                                }
                            }

                        } // while ((bytesRead = br.Read(buffer, 0, buffer.Length)) > 0)
                    }
                }
                finally
                {
                    if (fs != null)
                    {
                        fs.Dispose();
                    }
                }
            }

            return null;
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
