# Core.SiteInformation #

### Summary ###
This sample is a ready-built app that shows information about the current site collection with a "Site Information" custom action menu item.

The interface will load in a dialog box, showing info such as: site collection admins, storage quota & usage, the Webs count, last content modified date, sharing settings, etc. It is a useful starting point if you want to build something similiar for your users.

The sample also shows how to add a custom action under the Settings menu.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
The complete solution requires app registration with tenant permissions. Refer to AppRegNew.aspx and AppInv.aspx

### Solution ###
Solution | Author(s)
---------|----------
Core.SiteInformation | Radi Atanassov (OneBit Software)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 23rd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
The App has several sample components:
- An AppInstalled event receiver that registers the custom action
- A dialog box to show the interface
- a simple default.aspx page with code-behind logic to get the data

Sample interface:
![](http://i.imgur.com/fffDVVu.png)

Sample menu:
![](http://i.imgur.com/WavCqoC.png)

Most calls can only be performed with Site Collection permissions, however some require access to the tenant. Review the code to follow the required tenant permissions calls.

## Application Settings ##
Because the solution has two interface, application settings need to be configured in to places...the app.config of the console application project and the web.config of the MVC web app project. The follow code sample outlines the appSettings that need to be configured with values specific to your tenant/environment:

```XML
<appSettings>
  <!-- The client id and client secret of the app as provided in /_layouts/15/appregnew.aspx -->
  <add key="ClientID" value="YOUR_CLIENT_ID_FROM_APPREGNEW.ASPX" />
  <add key="ClientSecret" value="YOUR_CLIENT_SECRET_FROM_APPREGNEW.ASPX" />
  
  <!-- TenantName is the registered name for the Office 365 tenant such as onebitdev2015 -->
  <add key="TenantName" value="onebitdev2015" /> 
</appSettings>
```
