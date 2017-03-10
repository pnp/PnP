# File Upload CSOM SharePoint Add-In #

### Summary ###
This simple sample shows how to upload a large file into a SharePoint Library and Folder using client side object model from within a SharePoint add-in.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

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
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget package update
1.0  | May 8th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
This scenario shows uploading files to SharePoint using client side object model.  The first scenario will upload a document to a library, the second scenario will upload a document to a folder within the library. This sample solution uses an extension method UploadDocumentToLibrary & UploadDocumentToFolder defined in the FileFolderExtensions in the OfficeDevPnP Core project.

# SCENARIO: UPLOAD A FILE TO LIBRARY USING CSOM #
This scenario shows how to upload a file larger than 2MB to a SharePoint library using CSOM from a SharePoint application.  We will upload a document host web to new library called Docs, which will be created if it doesn’t exist.

```C#
string libraryName = "Docs";
List library = null;

// create library if it not exists
if (!ctx.Web.ListExists(libraryName))
{
    ctx.Web.CreateDocumentLibrary(libraryName);
}

// get the root folder
library = ctx.Web.Lists.GetByTitle(libraryName);
ctx.Load(library, l => l.RootFolder); 
ctx.ExecuteQuery();

// upload the file
library.RootFolder.UploadFile("SP2013_LargeFile.pptx", HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), true);
```

The below code shows the implementations code of the `UploadFile` extension method:

```C#
var newFileInfo = new FileCreationInformation()
{
    ContentStream = stream,
    Url = fileName,
    Overwrite = overwriteIfExists
};

var file = folder.Files.Add(newFileInfo);
folder.Context.Load(file);
folder.Context.ExecuteQueryRetry();
```

# SCENARIO: UPLOAD A FILE TO FOLDER USING CSOM #
This scenario shows how to upload a file larger than 2MB to a folder in a SharePoint library using CSOM from a SharePoint application.  We will upload a document to a host web to a hidden folder called “hiddentest”. We will create the folder if it doesn’t already exist. Since the folder is not visible from the browser UI, you may navigate to the folder using the URL in your browser.

```C#
string folderName = "hiddentest";
if (!ctx.Web.DoesFolderExists(folderName))
{
    ctx.Web.Folders.Add(folderName);
}

// Upload document to the folder
var destinationFolder = ctx.Web.Folders.GetByUrl(folderName);

// upload the file to the folder
destinationFolder.UploadFile("SP2013_LargeFile.pptx", HostingEnvironment.MapPath(string.Format("~/{0}", "Resources/SP2013_LargeFile.pptx")), true);
```

This is using the same `UploadFile` method as in the previous scenario.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.FileUpload" />