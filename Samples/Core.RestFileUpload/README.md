# Core.RestFileUpload

This sample demonstrate how to upload files to SharePoint using REST APIs.

Despite the upload methods described in [Large Files Upload](https://github.com/OfficeDev/PnP/tree/dev/Samples/Core.LargeFileUpload) link, the best approach for large files upload inside an add-in is to use SharePoint REST APIs.

Using the REST approach, you do not need to slice your file into pieces and can send a file up to 2GB.

This method is in compliance with SharePoint Online.


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
