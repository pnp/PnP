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

# UPLOAD LARGE FILES TO SHAREPOINT USING CSOM 1 #
This scenario shows how to upload large files to SharePoint using CSOM. There are different approaches which has size limitations.

## FILE SIZE LIMIT APPROACH ##
When using this approach you will receive an exception if file size is larger than 2 MB (2097152 bytes). This is due to the way the information is sent from client to the server.  The issue is caused by the fact that we are using the Content property of the *FileCreationInformation* object.

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
This alternative method, uses the *ContentStream* property of the *FileCreationInformation* object. This is valid approach for uploading large files sizes to SharePoint. 

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
This is an alternative method, which uses *SaveBinaryDirect* method File object. This is a valid approach for uploading large file sizes to SharePoint.

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

## LARGE FILE HANDLING - OPTION 3 (sliced upload) ##
This alternative uses the sliced upload capability of SharePoint: you can "chop" a large file in smaller slices of data and upload slice per slice. If somehow the upload get's stopped due to a network issue or a simply a user canceling an upload this API allows to "restart" the upload as of the last successfully uploaded slice. The **sample shown here deliberately is a "simple" one** to focus on the API: in a real life example you would read the bytes in a chunked way from the file system and you would persist state after each slice was uploaded. Below code shows a snippet from the `UploadFileSlicePerSlice`method: this method will read the file bytes in memory and then create a list of 'slices" e.g. a 2.5 MB file would mean 3 slices if the slice size is 1 MB.

```C#
// upload slice per slice. They'll need to be uploaded in the correct order
int sliceNumber = 0;
foreach (byte[] slice in sliceData)
{
    UploadFileSlice(ctx, uploadId, slice, docs.RootFolder, uniqueFileName, sliceNumber, sliceCount);
    sliceNumber++;
}
```

These slices are fed into the `UploadFileSlice` method:
```C#
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
```

The important elements in above sample are:
1. For the **first** slice first create an empty file and then use the `StartUpload` method. Store the bytesuploaded as that will the insertion point for the next slice of data.
2. For **all next but the last slice** call the `ContinueUpload` method. Store the bytesuploaded as that will the insertion point for the next slice of data.
3. For the **last** slice call the `FinishUpload` method




