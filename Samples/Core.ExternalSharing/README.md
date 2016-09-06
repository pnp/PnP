# External Sharing APIs for SharePoint and OneDrive for Business (Core.ExternalSharing) #

### Summary ###
This sample demonstrates how to user external sharing APIs with SharePoint or OneDrive for Business sites in Office 365. With external sharing APIs, you can automate document or site sharing for external users. With site level sharing, you can assign external user with view, edit or owner permissions. 

With document sharing you have few more options with the APIs.
* You can get anonymous link with view or edit permissions - by using link, there's no need to authenticate to access document. This link can have automated expiration time. 
* You can get link which requires authentication for users who you want to share the document with view or edit permissions

External sharing has to be explicitly enabled in the tenant and also in the specific site collection level. These settings can be also modified using CSOM APIs, if you have sufficient permissions to current tenant. 

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
API is available from SharePoint sites and also from OneDrive for Business sites, meaning that you can use demonstrated file sharing capabilities on both services. Site level sharing only works on the SharePoint sites.

Example scenarios with these APIs

* Upload document and share that automatically for people to review
* Use Office 365 as the extranet platform by automatically provisioning site with needed branding and then share the site automatically for the external partners using now documented APIs

Actual sample is pretty simplistic and is meant to be used as "stepping stone" to understand how the external sharing APIs actually work. This way you can more easily adapt the models to your own solutions. When you start the provider hosted sample, you'll see following UI for different options.

![Add-in UI](http://i.imgur.com/zyZw9yx.png)

Sample app/add-in is using extension methods for simplifying the API calls from managed code. This means that as long as you add using statement for ffjfjfjf, you will see these additional methods for the Web object. Notice also that these extension are included in the [PnP Core component](https://www.nuget.org/packages/OfficeDevPnPCore16) starting from October 2015 release.

## FAQ ##
**Q** - Does this work with app/add-in only permissions?
**A** - Yes

**Q** - Does this work for files in OneDrive for Business?
**A** - Yes. APIs are working with documents from OneDrive for Business sites.

**Q** - Do I need to get app/add-in installed on site to use this?
**A** - No. Sample is using provider hosted add-in UI just for demonstration purposes, you can use Azure WebJobs or whatever model for running the managed code demonstrated in the sample.  

**Q** - Do I need to use app/add-in authentication for this?
**A** - No. You can use classic account and pwd model with SharePointOnlineCredentials for making this work.

**Q** - I can't make the API to work within file or site level - what's wrong?
**A** - Typical issue would be tenant or site collection level settings. Ensure that you have external sharing enabled at both tenatn and in the specific site collection level. You can use this sample app/add-in to access that information or alternatively use tenant SharePoint admin UIs for those configurations.

**Q** - This app/add-in is requesting farm permissions, can't I make any of this work with smaller permissions?
**A** - You can. Add-in is also accessing farm level settings for the tenant management, which is the reason for so high permissions. If you are only sharing sites or documents, you will need lower permissions depending on exact case. For site level sharing, you'll need to request Site Collection Owner permissions and for document sharing you'll need site manage permission. 

**Q** - What if I try to share a site or document wiht insufficient permissions?
**A** - You will get AccessRequestsQueued as the return value for the sharing request, which means that your request is queued for the site owner for verification. This is similar behaviour as with the UI based sharing using browser. See below picture for the the entry added in the queue for site owner processing.

![UI with pending request status](http://i.imgur.com/BQu8o48.png)


# Controlling tenant and site collection setting with CSOM
To be able to control tenant and site collections settings, you'll need to request Tenant Admin permissiosn for the add-in. After this you need to create context to your SharePoint administrations site with the url of "https://yourteannt-admin.sharepoin.com". This will give you needed permissions and acccess to Tenant level settings. 

Following code snippet shows the model for accessing tenant level settings around external sharing.

```C#
// Get site collections.
Tenant tenant = new Tenant(adminCtx);
SPOSitePropertiesEnumerable sites = tenant.GetSiteProperties(0, true);
ctx.Load(tenant);
ctx.Load(sites);
ctx.ExecuteQuery();

SharingCapabilities tenantSharing = tenant.SharingCapability;
switch (tenantSharing)
{
    case SharingCapabilities.Disabled:
        lblStatus.Text = "External sharing is disabled at tenant level.";
        break;
    case SharingCapabilities.ExternalUserSharingOnly:
        lblStatus.Text = "External sharing at tenant level is set only for authenticated users.";
        break;
    case SharingCapabilities.ExternalUserAndGuestSharing:
        lblStatus.Text = "External sharing at tenant level is for authenticated and guest users.";
        break;
    default:
        break;
}
```

You can also control this tenant level setting from the tenant administration UIs. Here's the setting under SharePoint Online admin UIs.

![Sharing option in tenant admin UI](http://i.imgur.com/uTpFunu.png)

By default all SharePoint site collection shave external sharing disabled, so you'll need to explicitly enable that as needed. You can easily automate this as part of your site collection provisioning tooling or manually by modifying *Sharing* settings from the SharePoint online tenant UIs. 

![Sharing button in the SharePoint site collection list](http://i.imgur.com/OSsfyus.png)

Here's the actual popup for changing sharing settings.

![Sharing options for specific site collection](http://i.imgur.com/UyAf6lI.png)

You can also adjust this setting using CSOM APIs. Here's a code snippet from the sample to adjust site collection level setting around sharing.

```C#
// Get site collection
Tenant tenant = new Tenant(ctx);
SiteProperties siteProp = tenant.GetSitePropertiesByUrl(siteUrl, true);
ctx.Load(siteProp);
ctx.ExecuteQuery();

switch (rblSharingOptions.SelectedValue)
{
    case "Disabled":
        siteProp.SharingCapability = SharingCapabilities.Disabled;
        lblStatus.Text = "External sharing is for authenticated and guest users.";
        break;
    case "ExternalUserAndGuestSharing":
        siteProp.SharingCapability = SharingCapabilities.ExternalUserAndGuestSharing;
        lblStatus.Text = "External sharing is for authenticated and guest users.";
        break;
    case "ExternalUserSharingOnly":
        siteProp.SharingCapability = SharingCapabilities.ExternalUserSharingOnly;
        lblStatus.Text = "External sharing is for authenticated and guest users.";
        break;
}
// Update based on applied setting
siteProp.Update();
ctx.ExecuteQuery();
```

# External sharing of sites with CSOM
Actual API for for site and document sharing is exactly the same (ShareObject), but this sample introduces slightly more simplified API for this usage, so that sharing can be more efficiently used. 

Original API for the site sharing as in MSDN - https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.web.shareobject.aspx. We'll update documentation of this API soon (currently Oct 2015), but it's pretty complex to use with undocumented values to be passed in. 

```C#
public static SharingResult ShareObject(
	ClientRuntimeContext context,
	string url,
	string peoplePickerInput,
	string roleValue,
	int groupId,
	bool propagateAcl,
	bool sendEmail,
	bool includeAnonymousLinkInEmail,
	string emailSubject,
	string emailBody
)

```

To make things simpler, we have included much more simplified API included in the sample (and in PnP core component) as extension method. You can simply share a site with providing an email address and the requested permission. You can also control email settings in the method signature and you do not need worry about other settings or constants to be passed for the native API.

```C#

SharingResult result = ctx.Web.ShareSite("someone@example.com", ExternalSharingSiteOption.Edit,
                                         true, "Here's a site shared for you.");

```

*Notice* - This capability is also available with RESTful interfaces.


# External sharing of documents with CSOM
Sample includes few different options for the document sharing, like shown in teh below picture.

![Add-in UI for sharing files](http://i.imgur.com/nPCX4ak.png)

Original API for the document sharing with authentication is the same API in the MSDN which is used for site sharing - https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.web.shareobject.aspx.

Simplified API included in the sample (and in PnP core component) as extension method.

```C#

SharingResult result = ctx.Web.ShareDocument("https://tenant.sharepoint.com/docs/file.xls", 
												"someone@example.com", ExternalSharingDocumentOption.Edit,
                                                true, "Here's your important document");

```

*Notice* - This capability is also available with RESTful interfaces.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.ExternalSharing" />