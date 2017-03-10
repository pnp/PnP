# Site Collection Provisioning #

### Summary ###
Demonstrates how to create site collections using CSOM for Office 365 from provider hosted add-in.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
Provisioning.SiteCollectionCreation | Vesa Juvonen, Frank Marasco, Bert Jansen - Microsoft

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | May 5th 2013 (to update) | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
The APIs for creating site collections, subsites, and OneDrive for Business sites are different. Only the on-demand pattern applies to OneDrive for Business sites, because the code that provisions personal sites must run under the identity of the user who owns the site. You can apply the other two patterns to creation of all other types of SharePoint sites. The [Self-Service Site Provisioning using Apps for SharePoint 2013](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2013/04/04/self-service-site-provisioning-using-apps-for-sharepoint-2013.aspx) sample by Richard diZerega demonstrates this by enabling creation of both subsites and site collections through a [customization form](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2013/04/04/self-service-site-provisioning-using-apps-for-sharepoint-2013.aspx). You may also visit Vesa "vesku" Juvonen blog for additional information  for [SharePoint 2013 site provisioning](http://blogs.msdn.com/b/vesku/archive/2014/03/02/sharepoint-online-solution-pack-for-branding-and-provisioning-released.aspx) techniques presentation video recording
This code only works on an Office 365 Multi-Tenant (MT) SharePoint site. This sample will not work in an on-premises installation of SharePoint. This sample will not work on Dedicated installation of SharePoint, but will work in a future update to the platform.


```C#
//get the base tenant admin urls
var tenantStr = hostWebUrl.ToLower().Replace("-my", "").Substring(8);
tenantStr = tenantStr.Substring(0, tenantStr.IndexOf("."));

//get the current user to set as owner
var currUser = ctx.Web.CurrentUser;
ctx.Load(currUser);
ctx.ExecuteQuery();

//create site collection using the Tenant object
var webUrl = String.Format("https://{0}.sharepoint.com/{1}/{2}", tenantStr, "sites", url);
var tenantAdminUri = new Uri(String.Format("https://{0}-admin.sharepoint.com", tenantStr));
string realm = TokenHelper.GetRealmFromTargetUrl(tenantAdminUri);
var token = TokenHelper.GetAppOnlyAccessToken(TokenHelper.SharePointPrincipal, tenantAdminUri.Authority, realm).AccessToken;

using (var adminContext = TokenHelper.GetClientContextWithAccessToken(tenantAdminUri.ToString(), token))
{
var tenant = new Tenant(adminContext);
       var properties = new SiteCreationProperties()
       {
       	Url = webUrl,
             	Owner = currUser.Email,
             	Title = title,
             	Template = template,
             	StorageMaximumLevel = 100,
             	UserCodeMaximumLevel = 100
       };

      //start the SPO operation to create the site
      SpoOperation op = tenant.CreateSite(properties);
      adminContext.Load(tenant);
      adminContext.Load(op, i => i.IsComplete);
      adminContext.ExecuteQuery();

      //check if site creation operation is complete
      while (!op.IsComplete)
      {
      		//wait 30seconds and try again
              System.Threading.Thread.Sleep(30000);
              op.RefreshLoad();
              adminContext.ExecuteQuery();
      }
}

```

# SOLUTION #
![Visual Studio Solution structure](http://i.imgur.com/6i04oFS.png)

SiteColectionCreation – SharePoint Application 

Because the add-in needs the ability to create sub-sites and site collections anywhere in the tenancy, it will need FullControl permission on the entire tenancy.  The add-in will also need to make add-in only calls to SharePoint, so it can work with tenant objects or sites outside the context.  Both these settings can be configured in the Permissions tab of the AppManifest.xml.

**NOTE**: You should typically avoid requesting tenancy permissions in your apps…especially with FullControl.  It is a best practice for apps to request the minimum permissions they need to function.  The “tenancy” permission scope is in place specifically for scenarios like provisioning.  


# SHAREPOINT ONLINE SETUP #
The first step to create the application principal. The add-in principal is an actual principal in SharePoint 2013 for the add-in that can be granted permissions.  To register the add-in principal, we will use the “_layouts/AppRegNew.aspx”. 

Now we need to grant permissions to the add-in principal.  You will have to navigate to another page in SharePoint which is the “_layouts/AppInv.aspx”. This is where you will grant the application Tenant permissions, so that our Site Provisioning application may create site collections.

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
 <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

# Dependencies #

- Microsoft.Online.SharePoint.Client.Tenant
- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- [Setting up provider hosted add-in to Windows Azure for Office365 tenant](http://blogs.msdn.com/b/vesku/archive/2013/11/25/setting-up-provider-hosted-app-to-windows-azure-for-office365-tenant.aspx)


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Provisioning.SiteColectionCreation" />