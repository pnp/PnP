# Large file upload with CSOM #

### Summary ###
This scenario shows the right approach for uploading files to the SharePoint using client side object model. There are few different ways to achieve this task and how to avoid the 2 MB file upload limit. 

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.LargeFileUpload | Vesa Juvonen, Bert Jansen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
2.0  | February 16th 2015 | Version 2 now supports the new "sliced" file upload API
1.0  | December 10th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# WHAT UPLOAD METHOD TO USE? #
This sample shows 4 different ways to upload files to SharePoint...but which method should you use and the "why" is captured in below table

Approach | Limitations | When to use | Platform
-------- | ----------- | ----------- | --------
**Content** property on the `FileCreationInformation` class | Max 2 MB file size, security time-out after 30 minutes | **Not recommended** to use this due to file size limitations | V15 on-premises + v16 (MT)
**ContentStream** property on the `FileCreationInformation` class | No file size limitations, but there's a security time-out after 30 minutes | **Recommended** for all v15 on-premises scenarios and for files < 10 MB in v16 (MT) | V15 on-premises + v16 (MT)
**SaveBinaryDirect** method on the `File` class | No file size limitations, but there's a security time-out after 30 minutes | **Not recommended** to use this because it does not work when used in apps or with add-in only authentication | V15 on-premises + v16 (MT)
**StartUpload, ContinueUpload and FinishUpload** methods on the `File` class | No file size limitations, but there's a security time-out after 30 minutes. If each data slice gets uploaded within 30 minutes there's no problem | **Recommended** for all v16 (MT) scenarios that deal with files > 10 MB | v16 (MT)

In the remaining chapters you'll find more details on above approaches

# UPLOAD LARGE FILES TO SHAREPOINT USING CSOM#
This scenario shows how to upload large files to SharePoint using CSOM. There are different approaches which has size limitations.

## FILE SIZE LIMIT APPROACH (Content) ##
When using this approach you will receive an **exception if file size is larger than 2 MB** (2097152 bytes). This is due to the way the information is sent from client to the server.  The issue is caused by the fact that we are using the Content property of the *FileCreationInformation* object.

```C#
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
```

## LARGE FILE HANDLING – OPTION 1 (ContentStream) ##
This alternative method, uses the *ContentStream* property of the *FileCreationInformation* object. This is valid approach for uploading large files sizes to SharePoint and the recommended approach when your on-premises. If you're in Office 365 this approach is recommended for files < 10 MB. Once you go beyond 10 MB using the new sliced upload (option 3 in this article) will be more reliable. This method will result in a security timeout after 30 minutes which you'll hit once you upload really large files. The sliced upload properly deals with this.

```C#
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

```

## LARGE FILE HANDLING – OPTION 2 (SaveBinaryDirect) ##
This is an alternative method, which uses *SaveBinaryDirect* method File object. This is a valid approach for uploading large file sizes to SharePoint, but not a recommended one. This approach **does not work when used from an add-in, nor does it support add-in only authentication** and will result in a security timeout after 30 minutes which you'll hit once you upload really large files. The sliced upload (option 3 in this article) properly deals with this.

```C#
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
```

## LARGE FILE HANDLING - OPTION 3 (StartUpload, ContinueUpload and FinishUpload) ##
This alternative uses the sliced upload capability of SharePoint: you can "chop" a large file in smaller slices of data and upload slice per slice. If somehow the upload get's stopped due to a network issue or a simply a user canceling an upload this API allows to "restart" the upload as of the last successfully uploaded slice. The **sample shown here deliberately is a "simple" one** to focus on the API: in a real life example you would persist state after each slice was uploaded and have a *retry* mechanism. Below code shows a snippet from the `UploadFileSlicePerSlice`method: this method will read the file in the specified blocksize and then upload this block to SharePoint. 

> Note that the method will fall back to the ContentStream approach (option 1) whenever the file is smaller then the defined blocksize.

The below snippet show the essential part of the sliced upload:

```C#
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

```

The important elements in above sample are:

1. For the **first** slice first create an empty file and then use the `StartUpload` method. Store the bytesuploaded as that will the insertion point for the next slice of data.
2. For **all next but the last slice** call the `ContinueUpload` method. Store the bytesuploaded as that will the insertion point for the next slice of data.
3. For the **last** slice call the `FinishUpload` method

When using the sliced upload you'll need to be aware of:
- Internal testing has shown that a **slice size of 8 MB works best**, but off course this depends on the network connection you're using
- When an upload is interrupted or stopped the server has an **unfinished file**. This file will be **cleaned up by the server after 6 to 24 hours**...this model is meant to immediately restart a failed upload, not restart it the day after. Keep in mind that that the current cleanup interval can be changed without upfront notice.
- Slices of data have to be **uploaded in the right order**, doing a parallel multi-threaded upload will not work
- When an upload starts the **file gets locked for 15 minutes**...if there's no new slice of data within that timeframe someone else can start an upload for that same file and your upload will be cancelled. Keep in mind that that the current file lock interval can be changed without upfront notice.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.LargeFileUpload" />