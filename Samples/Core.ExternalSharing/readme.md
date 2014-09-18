# Core.ExternalSharing #

### Summary ###
This sample demonstrates how to control the external sharing settings for a site collection in office 365 MT

### Applies to ###
-  Office 365 Multi Tenant (MT)


### Solution ###
Solution | Author(s)
---------|----------
Core.ExternalSharing | Frank Marasco (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | January 07, 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# GENERAL COMMENTS #
This sample shows how you can control the external sharing settings for a site collection in Office 365 MT. This “External sharing” capability is not available for on-premises and Office 365 dedicated deployments. In order to use external sharing it first needs to be enabled at tenant level: the code will check this like shown below:


```C#
SharingCapabilities _tenantSharing = _tenantAdmin.SharingCapability;

if(_tenantSharing == SharingCapabilities.Disabled)
{
  Console.WriteLine("Sharing is currently disabled in your tenant");
}

```

If external sharing is enabled at tenant level then you can control the external sharing setting via below code:


```C#
SiteProperties _siteprops = _tenantAdmin.GetSitePropertiesByUrl(siteCollectionURl, true);

_siteprops.SharingCapability = shareSettings;
_siteprops.Update();
adminCC.ExecuteQuery();


```

Where shareSettings is an enum:
```C#
public enum SharingCapabilities
{
  Disabled = 0,
  ExternalUserSharingOnly = 1,
  ExternalUserAndGuestSharing = 2,
}
```
