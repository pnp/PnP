# Core.RestFileUpload

This sample demonstrate how to upload files to SharePoint using REST APIs.

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
