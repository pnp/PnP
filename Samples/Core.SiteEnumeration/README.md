# Site collection enumeration #

### Summary ###
This sample shows how to use search to enumerate site collections.

*Notice*: This sample uses [PnP Core Nuget package](https://github.com/OfficeDev/PnP-sites-core) for the needed API operations.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
None

### Solution ###
Solution | Author(s)
---------|----------
Core.SiteEnumeration | Bert Jansen (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.2  | August 17th 2015 | Updated to use PnP Core as Nuget package
1.1  | August 5th 2015 | Nuget update
1.0  | December 20th 2013 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# SCENARIO NEED #
This scenario shows how one can use the SharePoint search engine to return a list of site collections. A list of site collections, including my site site collections, is needed for governance related tasks such as updating existing sites, building a report of existing my site site collections, …

In Office 365 Multi-Tenant there’s the tenant administration client side object model that offers site enumeration for regular team sites, but not for my sites. 

Since this solution is based on SharePoint search the following restrictions have to be taken in account:
-  Freshly created site collections might not yet show up
-  Site collections that are marked as “do not index” do not show up. Typical examples are the my site host site collection and the search center site collections

# HOW TO USE AGAINST OFFICE 365 MULTI-TENANT #
In a first step setup the client context object using the available methods in Office AMS Core. Note that the site URL must be set to the my site host site collection.

```C#
ClientContext cc = new AuthenticationManager().GetSharePointOnlineAuthenticatedContextTenant("https://bertonline-my.sharepoint.com", 
                    "user@bertonline.onmicrosoft.com", 
                    GetPassWord());
```

There a number of options to fetch a site collections list:

```C#
// Only lists the my sites
List<Site> sites = cc.Web.MySiteSearch();

// List all site collections
List<Site> sites = cc.Web.SiteSearch();

// Lists site collections scoped to an URL
List<Site> sites = cc.Web.SiteSearchScopedByUrl("https://bertonline.sharepoint.com");

// List site collections scoped by title
List<Site> sites = cc.Web.SiteSearchScopedByTitle("test");
```

Once the results are found Linq queries can be used to further filter and sort the results:

```C#
// if needed furhter refine the returned set of site collections
var bertSites = from p in sites
                where p.Url.Contains("kevin")
                select p;
```

# HOW TO USE AGAINST OFFICE 365 DEDICATED OR SHAREPOINT 2013 ON-PREMISES #
In a first step setup the client context object using the available methods in OfficeDevPnP Core. Note that the site URL must be set to the my site host site collection.

```C#
ClientContext cc = new AuthenticationManager().GetNetworkCredentialAuthenticatedContext("https://my.microsoft.com", 
                    "user", 
                    GetPassWord(), 
                    "europe");
```

The actual search options are the same as explained in the previous chapter.

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SiteEnumeration" />