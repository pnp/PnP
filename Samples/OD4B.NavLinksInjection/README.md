# OD4B Secondary Navigation Bar #

*Notice.* This solution **ONLY** works when you use so called "Classic" option for OneDrive for Business. New experiences do **NOT** support any level of branding in OneDrive for Business sites.

### Summary ###
This sample shows how to a secondary level of navigation under the suite bar on a OneDrive or to any other site. Typical business case would be to add custom navigation links to the OneDrive sites in hybrid situation where the end user is moving from on-premises to OneDrive for Business site and needs to find easily route back to on-prem sites. 

### Applies to ###
-  Office 365 Multi Tenant (MT) - Classic experience
-  Office 365 Dedicated
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
OD4B.NavLinksInjection | Brian Michely (Microsoft), Vesa Juvonen (Microsoft) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 24th 2014 | Adjusted to work with any sites and styles updated to work with add-in hub
1.0  | October 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #

This samples demonstrates the ability to add a secondary level of navigation to a OneDrive For Business site. It uses javascript injection and injects the HTML directly underneath the suite bar. This does not make modifications to the suite bar. 

This sample also uses HTML5 localstorage for caching a timeout key to reduce the number of calls made to load link data. This can be changed to use cookies as well.

Currently for the purposes of this sample, the links are static. With slight modifications this sample will pull links from a source that you implement. Following picture is showing the second level navigation in practice. "Intranet" and "Tools" text are coming from custom navigation.

![Custom toolbar](http://i.imgur.com/ZpJCYAi.png)

*Note. You should NOT modify actual suite bar (Office navigation bar), since that is not SharePoint specific and is used cross other services in Office 365 as well, like yammer and Delve. By adding the links as secondary level navigation, you clearly indicate that these are for SharePoint and end user will not get confused with the changes cross other services. *

Here's the logical design between on-premises and cloud in hybrid setup.

![High level process with 3 pointers](http://i.imgur.com/MYOsB4o.png)

1. Users can access OneDrive for Business hosted in the Office 365 with single sign on experience cross networks. This will provide the storage advantages and other improvements for the users
2. Internal sites can be still hosted in the on-premises, if needed. You can control the OneDrive links to point to cloud OneDrive for Business location with SP2013 SP1 version or newer
3. End users are seamlessly moved cross on-premises and cloud as long as the single sign on has been properly configured. This solution will help by providing the needed UI elements for easier navigation cross environments.

# Solution #

OD4B.NavLinksInjection – SharePoint Provider Hosted Application is demonstrating how to use JS injection model to add second level navigation to SharePoint site. This can be added automatically as part of the provisioning to normal sites or to One Drive for Business sites.

**NOTE:** *The elements the javascript for to make the injection can change over time and cause the secondary navigation to not be rendered. If this will happen, you will have to adjust the JS file accordingly. This is however recommended approach for avoiding custom master pages, which will cause much more significant maintenance challenges. *


# RUNNING THE SAMPLE #
Run the application and click the "Inject Secondary Navigation" button. This will add the jslink to inject the secondary navigation. Click the "back to site" link and then click on the OneDrive link in the suite bar. The secondary navigation bar should appear directly under the suite bar.

![Add-in UI](http://i.imgur.com/C2ryF1e.png)

# DEPENDENCIES 

None

<img src="https://telemetry.sharepointpnp.com/pnp/samples/OD4B.NavLinksInjection" />