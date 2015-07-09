# OfficeDevPnP.PowerShell Commands #

### Summary ###
This solution shows how you can build a library of PowerShell commands that act towards SharePoint Online. The commands use CSOM and can work against both SharePoint Online as SharePoint On-Premises.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
In order to build the setup project the Wix toolset needs to be installed. You can obtain this from http://wix.codeplex.com.
In order to generate the generate the Cmdlet help you need Windows Management Framework v4.0 you can download it from http://www.microsoft.com/en-us/download/details.aspx?id=40855

### Solution ###
Solution | Author(s)
---------|----------
OfficeDevPnP.PowerShell | Erwin van Hunen (**Knowit Reaktor Stockholm AB**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 18th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW TO USE AGAINST OFFICE 365 #
A build script will copy the required files to a folder in your users folder, called:
*C:\Users\<YourUserName>\Documents\WindowsPowerShell\Modules\OfficeDevPnP.PowerShell.Commands*

This will automatically load the module after starting PowerShell 3.0.
To use the library you first need to connect to your tenant:

```powershell
Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials (Get-Credential)
```

In case of an unattended script you might want to add a new entry in your credential manager of windows. 

![](http://i.imgur.com/6NiMaFL.png)
 
Select Windows Credentials and add a new *generic* credential:

![](http://i.imgur.com/rhtgL1U.png)
 
Now you can use this entry to connect to your tenant as follows:

```powershell
Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials yourlabel
```

## Commands ##
[Navigate here for an overview of all cmdlets and their parameters:](Documentation/readme.md)
