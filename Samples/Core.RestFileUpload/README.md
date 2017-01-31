# Uploading files using REST #

### Summary ###
This sample demonstrate how to upload files to SharePoint using REST APIs.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Core.RestFileUpload | Rodrigo Romano

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 9th 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------
# Introduction #
Several upload methods are described in [Large Files Upload](https://github.com/OfficeDev/PnP/tree/dev/Samples/Core.LargeFileUpload) link, but this sample shows how to use the SharePoint REST APIs to upload files smaller than 250mb. SharePoint Online has a file limit of 250mb for file getting upload in single call. If you want to upload file over 250 MB and you’re on SharePoint Online, you MUST use the sliced upload approach.  The issue here is not the security timeout, but rather a hard file size upload limit set in the Web Application properties to which the tenant has no access.  This setting is immutable, so you will not be able to upload file larger than 250 MB unless you use the chunked file approach.

## Console Application

### NuGet Packages
To create a solution from scratch, Open Visual Studio and create a new Windows Console Application Solution.

Then click on **Tools** menu, **NuGet Package Manager** and in **Package Manager Console** item.

Install below packages:

- Install-Package AppForSharePointOnlineWebToolkit 

### **App.Config** changes
Register your SharePoint add-in on your tenant and change the ClientID and ClientSecret config on app.config file.
```
 <appSettings>
    <add key="ClientID" value="ClientID" />
    <add key="ClientSecret" value="ClientSecret" />
  </appSettings> 
```

### Update the code
Register your SharePoint add-in on your tenant and change the ClientID and ClientSecret config on app.config file.
```C#
string url = "https://[tenant].sharepoint.com";
			
/// SharePoint Folder Relative Url
string folderUrl = "";

string filePath = "";

var fileUrl = UploadRest(url, folderUrl, filePath);
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.RestFileUpload" />