# Core.SiteInformation #

### Summary ###
This sample is a ready-built add-in that shows information about the current site collection with a "Site Information" custom action menu item.

The interface will load in a dialog box, showing info such as: site collection admins, storage quota & usage, the Webs count, last content modified date, sharing settings, etc. It is a useful starting point if you want to build something similiar for your users.

The sample also shows how to add a custom action under the Settings menu. It makes use of the add-in Installed Event Receiver to add the custom action. For this reason it requires a publically addressable URL if installing to Office 365, or Azure ServiceBus. See below for more details.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
The complete solution requires add-in registration with tenant permissions. Refer to AppRegNew.aspx and AppInv.aspx
In order to test the add-in without deploying the Core.SiteInformationWeb project to a publicly available URL, Azure ServiceBus with ACS authentication is required. See http://msdn.microsoft.com/en-us/library/office/dn275975(v=office.15).aspx for more information.

### Solution ###
Solution | Author(s)
---------|----------
Core.SiteInformation | Radi Atanassov (**OneBit Software**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 23rd 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------

# Overview #
The add-in has several sample components:
- An AppInstalled event receiver that registers the custom action
- A dialog box to show the interface
- a simple default.aspx page with code-behind logic to get the data

Sample interface:

![Site information UI](http://i.imgur.com/fffDVVu.png)


Sample menu:

![Custom menu option in site actions](http://i.imgur.com/WavCqoC.png)

Most calls can only be performed with Site Collection permissions, however some require access to the tenant. Review the code to follow the required tenant permissions calls.

## Application Settings ##
Because the solution has two interface, application settings need to be configured in the web.config of the web site application project.

```XML
<appSettings>
  <!-- The client id and client secret of the add-in as provided in /_layouts/15/appregnew.aspx -->
  <add key="ClientID" value="YOUR_CLIENT_ID_FROM_APPREGNEW.ASPX" />
  <add key="ClientSecret" value="YOUR_CLIENT_SECRET_FROM_APPREGNEW.ASPX" />
  
  <!-- TenantName is the registered name for the Office 365 tenant such as onebitdev2015 -->
  <add key="TenantName" value="onebitdev2015" /> 
</appSettings>
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SiteInformation" />