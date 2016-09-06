# Site permissions and external users #

### Summary ###
This sample shows how you can manipulate site collection administrators and work with external sharing in Office 365 MT.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.Permissions | Frank Marasco, Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | May 5th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# General comments #
This sample focuses on two scenarios:
-  Showing how you can easily manipulate site collection administrators using CSOM code
-  Showing how to get the external sharing status and external users of a site collection or tenant

# SCENARIO 1: WORKING WITH SITE COLLECTION ADMINISTRATORS #
This scenario is only using regular CSOM API’s and thus having site collection permissions on the site (e.g. your account is a site collection admin already) is needed in order to update the site collection administrators of that site. First step in the sample is creating a ClientContext object by a user with the proper permissions:

```C#
ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(
                    String.Format("https://{0}.sharepoint.com/sites/{1}", tenantName, siteName),
                    String.Format("{0}@{1}.onmicrosoft.com", userName, tenantName), 
                    password); 
```

Using that ClientContext object you can then get a list of the current site collection administrators or update the site collection administrators as is shown in below code snippets:

```C#
List<UserEntity> admins = cc.Web.GetAdministrators();

List<UserEntity> adminsToAdd = new List<UserEntity>();
adminsToAdd.Add(new UserEntity() { LoginName = "i:0#.f|membership|user@domain" });

cc.Web.AddAdministrators(adminsToAdd);

UserEntity adminToRemove = new UserEntity() { LoginName = "i:0#.f|membership|user@domain" };
cc.Web.RemoveAdministrator(adminToRemove);
```

If you to set the site collection administrators for site collections where you’re not already a site collection administrator then this is possible by creating a clientcontext object using a registered add-in with tenant level permissions as explained below.

## CLIENTCONTEXT BASED ON AN OAUTH TOKEN WITH TENANT LEVEL PERMISSIONS ##

```C#
// Use (Get-MsolCompanyInformation).ObjectID to obtain Target/Tenant realm: <guid>
//
// Manually register an add-in via the appregnew.aspx page and generate an add-in ID and 
// add-in Secret. The add-in title and add-in domain can be a simple string like "MyApp"
//
// Update the AppID in your worker role settings
//
// Add the AppSecret in your worker role settings 
//
// Manually set the permission XML for you add-in via the appinv.aspx page:
// 1/ Lookup your add-in via its AppID
// 2/ Paste the permission XML and click on create
//
// Sample permission XML:
// <AppPermissionRequests AllowAppOnlyPolicy="true">
//   <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
// </AppPermissionRequests>
//
// As you're granting tenant wide full control to an add-in the appsecret is as important
// as the password from your SharePoint administration account!
```

Once you’ve done that you can use below code to obtain a clientcontext object for this add-in:

```C#
ClientContext cc = new AuthenticationManager().GetAppOnlyAuthenticatedContext(
                    "https://tenantname-my.sharepoint.com/personal/user2", 
                    "<your tenant realm>", 
                    "<appID>", 
                    "<appsecret>");
```

The provided URL is for the site collection for which you do want to change the site collection administrators for. Once you’ve this client context you can use the same code as shown above, but now you can change the site collection administrators for any site collection, including the OneDrive for Business site collections that each of the Office 365 users have.

# SCENARIO 2: WORKING WITH EXTERNAL SHARING (OFFICE 365 MT ONLY) #
This scenario shows how to deal with external sharing: get the external sharing status of a site collection and get a list of external users for a site collection or for the complete tenant. Since these functionalities require the tenant CSOM libraries you need to create a ClientContext against the tenant admin site collection as shown below. The user account used here needs to be a tenant administrator account.

```C#
ClientContext ccTenant = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant(
                            String.Format("https://{0}-admin.sharepoint.com/", tenantName), 
                            String.Format("{0}@{1}.onmicrosoft.com", userName, tenantName), 
                            password);
```

Once the clientcontext is ready you can use below code to get the external sharing status and to get a list of external users.

```C#
ccTenant.Web.GetSharingCapabilitiesTenant(new Uri(String.Format("https://{0}.sharepoint.com/sites/{1}", tenantName, siteName)))

List<ExternalUserEntity> externalUsers = ccTenant.Web.GetExternalUsersForSiteTenant(new Uri(String.Format("https://{0}.sharepoint.com/sites/{1}", tenantName, siteName)));

List<ExternalUserEntity> externalUsers = ccTenant.Web.GetExternalUsersTenant();
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SitePermissions" />