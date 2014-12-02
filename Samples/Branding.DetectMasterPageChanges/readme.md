# Branding.ApplyBranding #

### Summary ###
This sample demonstrates how to check for changes to the default seattle.master master page in SharePoint Online.
This allows organisations who have chosen to get on the treadmill of custom master pages to automatically detect that the default master page has changed due to a service update via an integration test, this would typically be run as part of a nightly build.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)

### Prerequisites ###
none

### Solution ###
Solution | Author(s)
---------|----------
Branding.DetectMasterPageChanges  | Gavin Barron

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | November 14th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SETUP #
To run this sample first set create an App.config file based off the supplied App.config.sample and provided apropriate details.

Now you should be able to run the Unit Test **NOTE: The value of KnownHashOfSeattle may need to be updated.