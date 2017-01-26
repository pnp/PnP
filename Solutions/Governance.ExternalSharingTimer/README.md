# External Sharing Expiration Service #

### Summary ###
SharePoint Online makes it extremely easy to share sites and content with external users. For this reason, SharePoint Online has seen rapid adoption for many extranet scenarios and in OneDrive for Business. SharePoint Online provides administrators the tools to manage external sharing, including enabling/disabling sharing and visibility into external users within a site collection. External sharing is simple, secure, and extremely powerful.  However, once content is shared externally, it stays shared forever…or at least until it is manually revoked by a content owner or administrator. This solution helps implement expiration timers on external sharing in SharePoint Online. The solution will also give content owners easy methods to extend/revoke external user access.

The solution includes three projects:

- **Governance.ExternalSharingTimer** is a console application project meant to run as a "[timer job](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/04/07/sharepoint-timer-jobs-running-as-windows-azure-web-jobs.aspx)". It's purpose is to check for and manage expiration of external shares
- **Governance.ExternalSharingTimer.Data** is a database project that keeps track of external shares by site and any extends to external shares. For debugging, this can leverage LocalDB
- **Governance.ExternalSharingTimer.Web** is a MVC web project that provides an interface for users to revoke or extend an external share following an expiration warning email sent from the "timer job"

For a more thorough overview of the solution, see the blog post: [http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/08/30/expiring-external-user-sharing-in-sharepoint-online.aspx](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/08/30/expiring-external-user-sharing-in-sharepoint-online.aspx)

### Walkthrough Video ###

Comprehensive video of the solution in action:
[https://www.youtube.com/watch?v=ytHUGQClJgM](https://www.youtube.com/watch?v=ytHUGQClJgM)

"Cartoon short" to illustrate the solution concept:
[https://www.youtube.com/watch?v=hEdowSGREpo](https://www.youtube.com/watch?v=hEdowSGREpo)

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
The complete solution requires a database provisioned

### Solution ###
Solution | Author(s)
---------|----------
Governance.ExternalSharingTimer | Richard diZerega (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 30th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #
This solution has two components that interface with SharePoint Online:

1. A Console Application meant to execute as a daily "[timer job](http://blogs.msdn.com/b/richard_dizeregas_blog/archive/2014/04/07/sharepoint-timer-jobs-running-as-windows-azure-web-jobs.aspx)" to look for new external sharing and process warnings/expirations.
2. An MVC Web Application for end-users to view details of expiration warnings and act on them by extending or revoking the share.

The solution also uses an application database for tracking share extensions and add-in only tenant permissions to perform site collection management. Both of these components require up-front configuration explained in the next section.

# Setup and Execution #
This is a complex solution with a number of moving parts. This section outlines the primary configuration steps required to run the sample, including assembly dependencies, permission configuration, database deployment, and application settings.

## Dependencies ##
Code is having reference to following CSOM assemblies.

- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- Microsoft.Online.SharePoint.Client.Tenant.dll

## Permission Configuration ##
This solution uses a provider-hosted approach, but does not have a tradition SharePoint entry point and will execute outside the context of a user (ie - Add-In Only). Because of these two constraints, you must register the add-in in /_layouts/15/appregnew.aspx and then manually configure permissions for the add-in in /_layouts/15/appinv.aspx:

Permission Request XML:

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
    <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```

Example of Configuration in /_layouts/15/AppInv.aspx
![Permissions provided for the app with appinv.aspx page](http://i.imgur.com/rhfQQQh.png)

## Database Deployment ##
For the purpose of debugging, the solution can leverage Local DB, which is included with Visual Studio. Complete the following steps to deploy the solution:

1. Right-click the **Governance.ExternalSharingTimer.Data** project and select "**Publish**"
2. In the "**Target database connection**" field of the **Publish Database** dialog, select the "**Edit**" button to launch the **Connection Properties** dialog
3. Provide the following details in the **Connection Properties** dialog and click the "**Ok**" button
       - Server name: **(localdb)\v11.0**
       - Select or enter a database name: **ExternalSharingData**
4. Finally, click the **Publish** button in the **Publish Database** dialog to publish the database to local DB
![Publish Database UI](http://i.imgur.com/roSHnoJ.png)

Both the configuration files (console app and MVC app) are pre-configured to use this connection information.

## Application Settings ##
Because the solution has two interface, application settings need to be configured in to places...the app.config of the console application project and the web.config of the MVC web add-in project. The follow code sample outlines the appSettings that need to be configured with values specific to your tenant/environment:

```XML
<appSettings>
  <!-- The client id and client secret of the add-in as provided in /_layouts/15/appregnew.aspx -->
  <add key="ClientID" value="YOUR_CLIENT_ID_FROM_APPREGNEW.ASPX" />
  <add key="ClientSecret" value="YOUR_CLIENT_SECRET_FROM_APPREGNEW.ASPX" />
  
  <!-- WarningDuration is the number of days before an expiration warning is sent out -->
  <add key="WarningDuration" value="50" />
  
  <!-- CutoffCuration is the number of days before an external user is revoked -->
  <add key="CutoffDuration" value="60" />
  
  <!-- TenantName is the registered name for the Office 365 tenant such as Contoso-->
  <add key="TenantName" value="contoso" />
  
  <!-- TenantUpnDomain is the registered UPN for the users...default is TENANTNAME.onmicrosoft.com -->
  <add key="TenantUpnDomain" value="contoso.onmicrosoft.com" />
</appSettings>
```

# Running the Solutions #
The solution is hard-coded to look through a specific array of site collections. This could easily be changed to iterate tenant site collections using other PnP samples, but was not the focus of this solution. You can add your own site collections to the array at the top of the Program.cs file in the console application project.

Debug the console application project first (Governance.ExternalSharingTimer) to populate the application database with external sharing details. If necessary, tweak the WarningDuration and CutoffDuration configuration settings to trigger the desired outcome(s) for testing.

Once a warning has been reached, the solution will send a warning email through SharePoint using CSOM. This email will contain links to view warning details or extend/revoke the share. To view these, start debugging of the MVC web application (Governance.ExternalSharingTimer.Web)

<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.ExternalSharingTimer" />