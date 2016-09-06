# ONEDRIVE FOR BUSINESS BRANDING CUSTOMIZATION #

### Summary ###
This sample demonstrates how to access an end-user's OneDrive for Business site and apply custom branding to it automatically.

> Note. This only works with Classic mode and not with modern OneDrive for Business experiences.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Solution ###
Solution | Author(s)
---------|----------
Contoso.Core.OneDriveCustomizer | Vesa Juvonen, Bert Jansen, Frank Marasco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# GENERAL COMMENTS #
This sample demonstrates how to access an end-user's OneDrive for Business site and apply custom branding to it automatically. The sample accesses the OneDrive for Business site using the social .NET client-side model (CSOM), which provides read access to user profile properties as well as access to the Site object for the OneDrive for Business site.

Branding is applied by uploading a custom them to the Site object by using file upload mechanisms in the CSOM and then applying the theme to the site.
You may also visit [Vesa "vesku" Juvonen's](http://blogs.msdn.com/b/vesku/archive/2013/11/25/office365-apply-automatically-custom-branding-to-personal-site-skydrive-pro.aspx) blog for addition information about the techniques in his video recording.

__Note:__
In general, Microsoft recommends branding OneDrive for Business sites using themes, and avoiding custom master pags. If you customize master pages, you'll have to ensure that any new changes on the out-of-the-box master pages are reflected on custom master pages as well. On top of the themes, you can also inject custom CSS to the site to modify layout slightly without needing to change the master page.

Branding is applied from within add-in part in the solution. You can place the add-in part anywhere in the tenant since it operates across the site collections as needed. When the add-in part is accessing the OneDrive for Business site, the end-user will only see a GIF animation that indicates when add-in part is accessing the personal OneDrive for Business site. These locations are typical for this customizer:

-  Intranet front page - When users arrive to Intranet, branding in OneDrive is checked and applied if necessary
-  Public side of the personal - for example on the news feed page

## SCENARIO: BRANDING THE ONEDRIVE SITE ##
This scenario demonstrates how to access the user profile, create the personal sites, and modify the site. We are using the Orange theme that is already available. Themes are not visible in the UI of OneDrive for Business sites, but you can still use them as well as deploy custom a custom theme. See the AMS sample CustomCSS and DeployCustomThemeWeb for additional information.

```C#
// Get user profile
ProfileLoader loader = Microsoft.SharePoint.Client.UserProfiles.ProfileLoader.GetProfileLoader(clientContext);
UserProfile profile = loader.GetUserProfile();
Microsoft.SharePoint.Client.Site personalSite = profile.PersonalSite;

clientContext.Load(personalSite);
clientContext.ExecuteQuery();

// Let's check if the site already exists.
// The following code uses a timer job-based approach to schedule the creation
// of a OneDrive for Business site if it has not yet been created for a particular user.
if (personalSite.ServerObjectIsNull.Value)
{
	profile.CreatePersonalSiteEnque(true);
    clientContext.ExecuteQuery();
}
else
{
	Web rootWeb = personalSite.RootWeb;
	clientContext.Load(rootWeb);
	clientContext.ExecuteQuery();
	
	// Setting the custom theme to host web yes its orange
	SetThemeBasedOnName(clientContext, rootWeb, "Orange");
}
```

## SHAREPOINT ONLINE SETUP ##
The first step to create the application principal. The add-in principal is an actual principal in SharePoint 2013 for the add-in that can be granted permissions.  To register the add-in principal, we will use the “_layouts/AppRegNew.aspx”. 

Now we need to grant permissions to the add-in principal.  You will have to navigate to another page in SharePoint which is the “_layouts/AppInv.aspx”. This is where you will grant the application Tenant permissions, so that our Site Provisioning application may create site collections.

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
 <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.OneDriveCustomization" />