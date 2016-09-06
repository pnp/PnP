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
Several upload methods are described in [Large Files Upload](https://github.com/OfficeDev/PnP/tree/dev/Samples/Core.LargeFileUpload) link, but this sample shows how to use the SharePoint REST APIs to upload files. Using the REST approach, you do not need to slice your file into pieces and can send a file up to 2 GB, but the same security time-out restrictions mentioned in the large upload sample apply here. If you really want to upload large files and you're on SharePoint Online the sliced upload is the advised approach. For on-premises or for files that can be uploaded within the security timeout window the REST approach is good one.

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