# OneDrive Provisioning #

### Summary ###
As part of the new Client Side Object Model (CSOM) assemblies released in the [SharePoint Online Client Components SDK](http://www.microsoft.com/en-us/download/details.aspx?id=42038), we now have many new capabilities and improvements. One specifically is the capability to programmatically provision OneDrive for Business sites in Office 365.

There is basically two ways to provision a user’s OneDrive for Business site, the first way is user initiated that is when the user navigates to their newsfeed, site or OneDrive links that are within the suite bar, the other is by pre-provisioning leveraging CSOM. In some cases it might not be feasible to have the user initiate the provision process. Say for example, you are migrating from an on-premises SharePoint farm or other repositories, and you don’t want to wait for the user to click a link before you start your migration. 

### Walkthrough Video ###

Visit the video on Channel 9 [http://channel9.msdn.com/Blogs/Office-365-Dev/Branding-OneDrive-for-Business-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practice](http://channel9.msdn.com/Blogs/Office-365-Dev/Branding-OneDrive-for-Business-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practice)

![http://channel9.msdn.com/Blogs/Office-365-Dev/Branding-OneDrive-for-Business-with-an-App-for-SharePoint-Office-365-Developer-Patterns-and-Practice](/Solutions/Provisioning.OneDrive/images/ch9scrnsht.png)

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.OneDriveProvisioning | Vesa Juvonen, Bert Jansen, Frank Marasco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------


### Additional references ###
For more information on this scenario, see also following blog post: 
[http://blogs.msdn.com/b/frank_marasco/archive/2014/03/25/so-you-want-to-programmatically-provision-personal-sites-one-drive-for-business-in-office-365.aspx](http://blogs.msdn.com/b/frank_marasco/archive/2014/03/25/so-you-want-to-programmatically-provision-personal-sites-one-drive-for-business-in-office-365.aspx)

## Dependencies ##
Code is having reference to following CSOM assemblies.

- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- Microsoft.SharePoint.Client.UserProfiles


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.OneDriveProvisioning" />