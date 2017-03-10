# SharePoint Online Site Usage Report #

### Summary ###
This solution is a console application designed to run periodically as a scheduled task. It will get data for all site collections in a tenant and send an email with key statistics like used storage quota, the last content modification date, the count of the webs.

The example could be built upon in a governance scenario, where the governance team can monitor how site collections are used.

The demonstration shows how you can run a console program periodically, without any user interaction. In a similiar way the console applicaiton could be replaced with a windows service to perform unattended operations to your tenant data. The code logic makes use of the add-in only policy to perform calls "without any user". In my example I have used the Tenant permission scope with FullControl rights.  

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
The complete solution requires add-in registration with tenant permissions

### Solution ###
Solution | Author(s)
---------|----------
Governance.SiteUsageReport | Radi Atanassov (OneBit Software)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 1st 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
The solution has one Visual Studio project - Governance.SiteUsageReport, which is a console application. It is intended that this application is to be ran as a scheduled task, however it could be easily replaced with a windows service if you prefer that design.

In this example the code logic builds an e-mail with a simple table of all site collections and data about them. This could be easily replaced with something else, such as generating an Excel document with OpenXml or anything else you would prefer.

One interesting aspect of the demonstration is that there is no "SharePoint add-in" project, no add-in package file. It uses the add-in registration pages to acquire a ClientId and ClientSecret, but there is no deployment to the tenant.

The other interesing element is the use of TokenHelper.cs. At the time of authoring v1, it has to be copied to the project and all of its needed references must be resolved to compile.

# Setup and Execution #
This solution is kept as simple as possible to illustrate the key elements. There is no add-in package, you just need to register a ClientId and ClientSecret and provide appropriate permissions.

## Dependencies ##
The project has references to the following assemblies:

- Microsoft.SharePoint.Client.dll
- Microsoft.SharePoint.Client.Runtime.dll
- Microsoft.Online.SharePoint.Client.Tenant.dll
- System.Configuration (part of the framework, used to read appSettings in the app.config file)

To compile TokenHelper.cs, the project also needs the following assemblies:
- Microsoft.IdentityModel.dll (I grabbed it with the Nuget package, "Install-Package Microsoft.IdentityModel")
- Microsoft.IdentityModel.Extensions.dll (You can grab it with the Nuget package if not resolved in your environment "Install-Package Microsoft.IdentityModel.Extensions")
- System.IdentityModel (part of the framework)
- System.ServiceModel (part of the framework)
- System.Web (part of the framework)
- System.Web.Extensions (part of the framework)

The project also references OfficeDevPnP.Core to use the AppModelExtensions (Tenant) and the SiteEntity object.

## Permission Configuration ##
This solution uses a provider-hosted approach, but does not have a tradition SharePoint add-in entry point and will execute periodically with no user interaction (ie - add-in Only). Because of these two constraints, you must register the add-in in /_layouts/15/appregnew.aspx and then manually configure permissions for the add-in in /_layouts/15/appinv.aspx:

Permission Request XML:

```XML
<AppPermissionRequests AllowAppOnlyPolicy="true">
    <AppPermissionRequest Scope="http://sharepoint/content/tenant" Right="FullControl" />
</AppPermissionRequests>
```
Start by getting and registering a ClientId and ClientSecret through /_layouts/15/AppRegNew.aspx
![Creation of ID and secret](http://i.imgur.com/qjzXtwD.png)
Make sure you copy the ClientId and ClientSecret and plug them in the app.config. Notice how the add-in Domain and Redirect URI are not required in this case beacuse we have no URL interaction

When done you will get a confirmation message:
![Confirmation on created entry](http://i.imgur.com/sWwsXDk.png)

Continue by giving it permissions through /_layouts/15/AppInv.aspx
![UI for providing permissions](http://i.imgur.com/F7TloiO.png)

You will then be asked to confirm the trust and assign the add-in permissions:
![Accept the given permissions for the app](http://i.imgur.com/s1G5MNX.png)


## Application Settings ##
Because the solution has two interface, application settings need to be configured in to places...the app.config of the console application project and the web.config of the MVC web add-in project. The follow code sample outlines the appSettings that need to be configured with values specific to your tenant/environment:

```XML
<appSettings>
  <!-- The client id and client secret of the add-in as provided in /_layouts/15/appregnew.aspx -->
  <add key="ClientID" value="YOUR_CLIENT_ID_FROM_APPREGNEW.ASPX" />
  <add key="ClientSecret" value="YOUR_CLIENT_SECRET_FROM_APPREGNEW.ASPX" />
  
  <!-- TenantName is the registered name for the Office 365 tenant such as onebitdev2015 -->
  <add key="TenantName" value="onebitdev2015" /> 
  
  <!-- TenantUpnDomain is the registered UPN for the users...default is TENANTNAME.onmicrosoft.com -->
  <add key="TenantUpnDomain" value="onebitdev2015.onmicrosoft.com" /> 

  <!-- TargetEmail is the email to which to send emails -->
  <add key="TargetEmail" value="admin@onebitdev2015.onmicrosoft.com" />
</appSettings>
```
<img src="https://telemetry.sharepointpnp.com/pnp/solutions/Governance.SiteUsageReport" />