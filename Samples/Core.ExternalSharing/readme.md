# External Sharing APIs for SharePoint and OneDrive for Business (Core.ExternalSharing) #

### Summary ###
This sample demonstrates how to control the external sharing settings for a site collection in office 365 MT.

Notice that even though this sample is using managed CSOM API, same API is avaialble throuhg REST for both SharePoitn and OneDrive for Business sites.

### Applies to ###
-  Office 365 Multi Tenant (MT)

### Solution ###
Solution | Author(s)
---------|----------
Core.ExternalSharing | Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | October 10, 2015 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# Introduction and possible use cases #
API is available from SharePoint sites and also from OneDrive for Business sites, meaning that you can use demonstrated file sharing capabilities on both sides. Site level sharing only works on the SharePoint side.

Possible scenarios with these APIs

* Upload document and share that automatically for people to review
* Use Office 365 as the extranet platform by automatically provisioning site with needed branding and then share the site automatically for the external partners using now documented APIs


## FAQ ##
**Q** - Does this work with app/add-in only permissions?
**A** - Yes

**Q** - Does this work for files in OneDrive for Business?
**A** - Yes. APIs are working with documents from OneDrive for Business sites.

**Q** - Do I need to get app installed on site to use this?
**A** - No. Sample is using provider hosted add-in UI just for demonstration purposes, you can use Azure WebJobs or whatever model for running the managed code demonstrated in the sample.  

**Q** - Do I need to use app/add-in authentication for this?
**A** - No. You can use classic account and pwd model with SharePointOnlineCredentials for making this work.

**Q** - I can't make the API to work within file or site level - what's wrong?
**A** - Typical issue would be tenant or site collection level settings. Ensure that you have external sharing enabled at both tenatn and in the specific site collection level. You can use this sample app to access that information or alternatively use tenant SharePoint admin UIs for those configurations.

**Q** - This app is requesting farm permissions, can't I make any of this work with smaller permissions?
**A** - You can. App is also accessing farm level settings for the tenant management, which is the reason for so high permissions. If you are only sharing sites or documents, you will need lower permissions depending on exact case. For site level sharing, you'll need to request Site Collection Owner permissions and for document sharing you'll need site manage permission. 

**Q** - What if I try to share a site or document wiht insufficient permissions?
**A** - You will get AccessRequestsQueued as the return value for the sharing request, which means that your request is queued for the site owner for verification. This is similar behaviour as with the UI based sharing using browser. See below picture for the the entry added in the queue for site owner processing.

![](http://i.imgur.com/BQu8o48.png)


# Controlling tenant and site collection setting with CSOM
To be

```C#
SharingCapabilities _tenantSharing = _tenantAdmin.SharingCapability;

if(_tenantSharing == SharingCapabilities.Disabled)
{
  Console.WriteLine("Sharing is currently disabled in your tenant");
}

```
SharePoint site sharing is controlled 

![](http://i.imgur.com/uTpFunu.png)

You will also need to enable explicitly at the site collection level

![](http://i.imgur.com/OSsfyus.png)

Actual settings are exposed with popup opened

![](http://i.imgur.com/UyAf6lI.png)


# External sharing of sites with CSOM

note. this capability is also available with RESTful interfaces.


# External sharing of documents with CSOM


note. this capability is also available with RESTful interfaces.

# GENERAL COMMENTS #
This sample shows how to take advantage of external sharing APIs in the SharePoint sites or in the OneDrive for Business sites. 




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
