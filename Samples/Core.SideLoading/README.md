# Side loading of add-ins/apps #

### Summary ###
This scenario shows how one can use side load app to SharePoint site. App can be either SharePoint hosted add-in or provider hosted add-in/app. Reference sample is done with SP hosted add-in. 

See following resources for additional details
- [SideLoading Guideance](http://blogs.msdn.com/b/frank_marasco/archive/2014/07/26/side-loading.aspx) - MSDN blog
- [Automating add-in/app installation to SharePoint sites using CSOM](http://blogs.msdn.com/b/vesku/archive/2015/11/20/automating-add-in-app-installation-to-sharepoint-sites-using-csom.aspx) - MSDN blog
- [How to install add-in/app to SharePoint sites using CSOM](https://channel9.msdn.com/blogs/OfficeDevPnP/How-to-install-add-inapp-to-SharePoint-sites-using-CSOM) - Channel 9 PnP video blog


### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.SideLoading | Frank Marasco (**Microsoft**), Vesa Juvonen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | July 27th 2014 | Initial release
1.1  | November 18th 2015 | Updated to use PnP Nuget package and some polishing

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Overview #
This scenario shows how one can use sideLoading of a Provider Hosted Application to install a SharePoint Provider Hosted Application to a site collection. SharePoint Administrators can deploy apps to their tenancy basically two different ways. Deploy from the add-in catalog (“app stapling”) or via sideloading. What is sideloading? add-in sideloading, from a SharePoint context, is the ability to install a SharePoint add-in directly into a site to explicitly bypass the regular governance controls of SharePoint. Sideloading apps is insecure. The main reason for blocking sideloading by default on non-developer sites is the risk that faulty apps pose to their host web/host site collection. Apps have the potential to destroy data and make sites or, given enough permissions, can even make site collections unusable. Therefore, apps should only be sideloaded in dev/test environments and in production only when deploying from the AppCatalog does not meet your needs. It is not recommended to sideload SharePoint Hosted-Applications, because of the risk of data loss.

***Note***
- Enabling the add-in sideloading features requires tenant admin permissions (in a multi-tenant environment) or farm admin permissions (in a single tenant environment or on-premises). 
- You must have a user context when sideloading the application. Add-in only permission is not available.
- Sideloading does not suppress the security check or compensate existing security requirements. It does however enable the programmatic installation of an add-in
- You must still register and add-in principal for SharePoint Provider hosted applications
- You should deactivate the sideloading feature immediately once the add-in is in installed. Site Collections administrators can install apps using CSOM which could circumvent your governance practices.


## Permissions ##

- Tenant: FullControl 


## Centrally Deployed Apps VS Sideloading comparison ##

- ADD-IN STAPLING (Deploy from the add-in catalog)  
	- Custom actions and add-in parts are not supported 
	- Add-in Install, Uninstall and upgrade event receivers cannot be handled
	- Site Collection Administrators cannot uninstall the application
	- Applied to new and existing site collections
	- There is metadata about the add-in and updates are applied

----------
	
-  Sideloading (Installing Provider hosted applications programmactically)
	- Custom actions and add-in parts are supported
	- Add-in Install, Uninstall and upgrade event receivers do fire and can be handled
	- Site Collection Administrators can uninstall the application
	- Custom code must be used to install the application
	- Tenant Administrators must enable the sideloading feature prior to install the application and should be disabled after the application is installed
	- There is no metadata about the add-in and updates have to be managed manually

## How to use ##

Since this solution is sideloading a provider hosted application, the following should be taken in account:

- The user must be a tenant administrator in order to enable the SideLoading Feature
- The Provider hosted application has already been registered by the tenant administrator
- The Provider hosted application has been deployed to your hosting platform

```C#
Guid sideloadingFeature = new Guid("AE3A1339-61F5-4f8f-81A7-ABD2DA956A7D");
// Prompt for URL
string url = GetUserInput("Please provide URL for the site where app is being installed: \n");
// Prompt for Credentials 
Console.WriteLine("Enter Credentials for {0}", url);
string userName = GetUserInput("SharePoint username: ");
SecureString pwd = GetPassword();

// Get path to the location of the app file in file system
string path = GetUserInput("Please provide full path to your app package: \n");

// Create context for SharePoint online
ClientContext ctx = new ClientContext(url);
ctx.AuthenticationMode = ClientAuthenticationMode.Default;
ctx.Credentials = new SharePointOnlineCredentials(userName, pwd);

// Get variables for the operations
Site site = ctx.Site;
Web web = ctx.Web;

try
{
    // Make sure we have side loading enabled. 
    // Using PnP Nuget package extensions.
    site.ActivateFeature(sideloadingFeature);
    try
    {
        // Load .app file and install that to site
        var appstream = System.IO.File.OpenRead(path);
        AppInstance app = web.LoadAndInstallApp(appstream);
        ctx.Load(app);
        ctx.ExecuteQuery();
    }
    catch
    {
        throw;
    }
    // Disable side loading feature using 
    // PnP Nuget package extensions. 
    site.DeactivateFeature(sideloadingFeature);
}
catch (Exception ex)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine(string.Format("Exception!"), ex.ToString());
    Console.WriteLine("Press any key to continue.");
    Console.Read();
}
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SideLoading" />