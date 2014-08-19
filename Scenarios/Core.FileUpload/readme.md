# File Upload CSOM SharePoint App #

### Summary ###
This simple sample shows how to upload a large file into a SharePoint Library and Folder using client side object model from within a SharePoint App.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D) 
-  SharePoint 2013 on-premises

#### Note: ####
Requires small adjustments for Office 365 Dedicated (D) and SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.FileUpload | Vesa Juvonen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 8th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
This scenario shows uploading files to SharePoint using client side object model.  The first scenario will upload a document to a library, the second scenario will upload a document to a folder within the library. This sample solution uses an extension method UploadDocumentToLibrary & UploadDocumentToFolder defined in the FileFolderExtensions in the OfficeDevPnP Core project.

# SCENARIO: UPLOAD A FILE TO LIBRARY USING CSOM #
This scenario shows how to upload a file larger than 2MB to a SharePoint library using CSOM from a SharePoint application.  We will upload a document host web to new library called Docs, which will be created if it doesn’t exist.

```C#
ctx.Web.UploadDocumentToLibrary(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), "Docs", true);
```

The below code shows the implementations code of the solution

```C#
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
```

# SCENARIO: UPLOAD A FILE TO FOLDER USING CSOM #
This scenario shows how to upload a file larger than 2MB to a folder in a SharePoint library using CSOM from a SharePoint application.  We will upload a document to a host web to a hidden folder called “hiddentest”. We will create the folder if it doesn’t already exist. Since the folder is not visible from the browser UI, you may navigate to the folder using the URL in your browser.

```C#
ctx.Web.UploadDocumentToFolder(HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), "hiddentest", true);
```

The below code shows the implementations code of the solution

```C#
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
```
