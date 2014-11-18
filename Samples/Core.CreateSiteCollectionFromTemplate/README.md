# Creating Site Collection using Site Template #

### Summary ###
This sample shows how you can create site collection from console application to the Office365 MT side, upload a site template to the site collection and apply the site template to the site collection. 

Notice that example template does time out pretty often when applied against Office365. Shows the pattern, but it’s recommended to use remote provisioning rather than custom site templates.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Solution ###
Solution | Author(s)
---------|----------
Core.CreateSiteCollectionFromTemplate | Ashish Trivedi (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | February 28th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Creating Site Collection using Site Template #
The following snippet applies a site template to a newly provisioned site collection:

    newWeb.ApplyWebTemplate(webtemp.Name);
    newWeb.Update();

 