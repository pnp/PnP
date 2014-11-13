# OD4B Secondary Navigation Bar #

### Summary ###
This sample shows how to a secondary level of navigation under the suite bar on a OneDrive. 

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
OD4B.NavLinksInjection | Brian Michely (Microsoft) 

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 6th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction #

This samples demonstrates the ability to add a secondary level of navigation to a OneDrive For Business site. It uses javascript injection and injects the HTML directly underneath the suite bar. This does not make modifications to the suite bar. 

This sample also uses HTML5 localstorage for caching a timeout key to reduce the number of calls made to load link data. This can be changed to use cookies as well.

Currently for the purposes of this sample, the links are static. With slight modifications this sample will pull links from a source that you implement. 


# Solution #

OD4B.NavLinksInjection – SharePoint Provider Hosted Application 

**NOTE:** *You should only use this as needed. The elements the javascript looks for to make the injection can change over time and cause the secondary navigation to not be rendered. This was initially done to provide links back to other web applications after a OneDrive migration*


# RUNNING THE SAMPLE #
Run the application and click the "Inject Secondary Navigation" button. This will add the jslink to inject the secondary navigation. Click the "back to site" link and then click on the OneDrive link in the suite bar. The secondary navigation bar should appear directly under the suite bar.

# DEPENDENCIES 

None