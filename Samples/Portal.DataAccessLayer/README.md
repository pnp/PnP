# Client-Side Data Access Layer (DAL) Sample #

## Summary ##
The Client-Side Data Access Layer (DAL) sample provides a reference implementation of the Client-Side Data Access Layer (DAL) Framework. The sample is intentionally verbose and rather straightforward in its implementation; the focus is on making the logic and operation easy to read and understand. 

The Client-Side Data Access Layer (DAL) Framework is a custom client-side JavaScript framework that you can make available to all of your custom client-side display controls. It supports intelligent data loading patterns, abstracts the details of the client-to-server requests, provides data caching functionality (with expiration) to minimize client-to-server request traffic, and improves perceived page performance. 

You can learn more about the DAL by reading the following article:

- [Proven Practices for SharePoint Online Portals - Performance](https://msdn.microsoft.com/en-us/pnp_articles/portal-performance)


### Applies to ###
-  SharePoint Online (all SKUs)
-  SharePoint 2013 On-Prem (Standard and Enterprise) [*not tested, but should work as expected*]

### Solution ###
Solution | Author(s)
---------|----------
Portal.DataAccessLayer | Ronald Tielke (**Microsoft**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 9th 2017 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**

----------
## Installation ##
The Client-Side Data Access Layer (DAL) sample must be installed in your SharePoint Online tenancy.  The sample includes an easy-to-use Console application that automates nearly the entire installation process.  However, some manual steps are required.

`Note: the Console application employs an "ensure" semantic; it will not overwrite existing DAL demo assets.`

### Overview ###
The installation is summarized as follows:

- Manually create two site collections (Admin and Demo).
- Use the console application to create a folder hive in the Style Library of the root site collection and upload all necessary resource files.
- Use the console application to configure the two site collections for the demo.
- Manually configure a Search Schema Managed Property.

### Pre-Requisites ###
- SharePoint Online (MT) Tenancy
- Account with permission to create site collections and manage Search schema via the SharePoint Admin Center 
	- (e.g., https://contoso-admin.sharepoint.com)
- Account with permission to manage lists within the root site collection of the Portal web application 
	- (e.g., https://contoso.sharepoint.com/)

### Prepare SharePoint Online ###
1. Go to **SharePoint Admin Center** | **Site Collections** and create an **Admin** site collection (e.g., /sites/admin).  This site will be used to host the various portal-level configuration lists for the DAL sample.
	- Use the **Team Site** template
	- Make note of the resulting Admin site url
2. Go to **SharePoint Admin Center** | **Site Collections** and create a **Demo** site collection (e.g., /sites/demo). This site will be used to demonstrate the operation of the DAL sample.
	- Use the **Publishing Portal** template
	- Make note of the resulting Demo site url
3. Visit the root site collection of the **Portal** web application (e.g., https://contoso.sharepoint.com/) and verify that it contains a document library named **Style Library**. This document library will serve as the **CDN** used to host the resource files for the DAL sample.

### Prepare the Sample ###
1. Open the **Portal.DataAccessLayer** solution in Visual Studio.
2. Edit the **configuration.js** file of the **JS** project folder and ensure that the following variables contain values appropriate to your environment:
	- PortalWebAppAbsoluteUrl = 'https://**contoso**.sharepoint.com';
	- PortalAdminSiteAbsoluteUrl = 'https://**contoso**.sharepoint.com/sites/**admin**';
	- PortalCdnUrl = "/style%20library/pnp";
	- StockTickerSymbol = "MSFT";
3. Edit the **Portal.DataAccessLayer.master** file of the **Master Pages** project folder and ensure that the **src** attribute is correct for all CDN file **script** tags within the **PnP MODs** block (*starting at line 22*).
	- `we use a custom master page only to simplify the demo; it is neither a technical requirement nor a recommended approach`

### Deploy the Sample ###
1. Verify that SharePoint Online has completed provisioning of the **Admin** and **Demo** site collections.
2. Open the **Portal.DataAccessLayer** solution in Visual Studio.
3. Start the Console application.
4. When prompted, specify the credentials of an O365 account that has the permissions listed in the **Pre-Requisites** section.
5. Execute the following operations in the specified order:
	1. Operation #1 - Configure CDN
	2. Operation #2 - Configure Admin Site Collection
	3. Operation #3 - Configure Demo Site Collection

`Wait approximately 30 mins for the search crawler to index the new sites/lists.`

### Verify the Managed Properties ###
Go to **SharePoint Admin Center** | **Search Administration** | **Manage Search Schema**.  Verify that the following **Managed Properties** exist:

- PnPPortalConfigKeyOWSTXT
- PnPPortalConfigValueOWSMTXT
- PnPPortalLinkTextOWSTXT
- PnPPortalLinkUrlOWSTXT

`Do not proceed until the Managed Properties appear in the Search Schema (this can take up to 30 mins).`

### Configure the Sortable Managed Property ###
Go to **SharePoint Admin Center** | **Search Administration** | **Manage Search Schema**. Choose *one* of the **RefinableInt[00-99]** Managed Properties and configure as follows:

- **Alias**: PnPPortalDisplayOrder
- **Mappings to crawled properties**: Include Content from all crawled properties
	- ows\_PnPPortalDisplayOrder
	- ows\_q\_NMBR\_PnPPortalDisplayOrder

`You do not need to edit any of the auto-generated Managed Properties associated with this sample.`

### Re-Index the Lists ###
At this point it is necessary to re-index the various configuration lists of the DAL sample in order to ensure that the configuration of the new **PnPPortalDisplayOrder** managed property takes effect. You must visit each list and manually trigger a re-indexing operation. Fortunately, an easier approach is to simply re-execute Operations #2 and #3 of the Console application:

1. Open the **Portal.DataAccessLayer** solution in Visual Studio.
2. Start the Console application.
3. When prompted, specify the credentials of an O365 account that has **Site Collection Administrator** permissions on the **Admin** and **Demo** site collections.
4. Execute the following operations in the specified order:
	1. Operation #2 - Configure Admin Site Collection
	2. Operation #3 - Configure Demo Site Collection

`Wait approximately 30 mins for the search crawler to re-index the sites/lists.`

### Visit the Demo Site Collection and Exercise the Demo ###
1. Navigate to the DAL web of the Demo site collection to see the DAL in action.
	- https://**DemoSiteUrl**/dal (e.g., https://contoso.sharepoint.com/sites/demo/dal)
2. Note: You can also navigate to the root web of the Demo site collection.
	- https://**DemoSiteUrl**/ (e.g., https://contoso.sharepoint.com/sites/demo)
3. The home page contains the following client-side JavaScript Controls:
	- **Global Nav**- managed via the **GlobalNavConfig** list of the Admin site
	- **Footer** - managed via the **PortalConfig** list of the Admin site
	- **Company Links** - managed via the **CompanyLinksConfig** list of the current **site collection**
	- **Local Nav** - managed via the **LocalNavConfig** list of the current **web** 
	- **Stock Ticker** - managed via the **StockTickerSymbol** variable in the **constants.js** file
	- **User Info** - managed via the **AAD record** and **user profile** of the current user
4. Press **F12** to launch the Internet Explorer Developer Tools.
5. Activate the **Console** window.
6. Reload the page and review the log messages sent to the **Console** window.
7. Note that the control data is being served from the client-side cache.
8. Note that the data for each control has a unique expiration timeout value and expiration policy.
9. Wait a few minutes for an expiration to occur.
10. Reload the page and review the log messages sent to the **Console** window.
11. Note that some control data has expired and that the back-end data source is called.
12. At any time, flush the DAL entries from the client-side cache by appending the following argument to the query string:
	- "**clearStorage=1**" 
13. At any time, edit the various management lists or data sources to customize the data presented by these controls. 
	- The controls will render the updated data in 15-20 mins, once the changes are crawled by SharePoint.
14. At any time, edit the various implementation files to customize the configuration of these controls. 
	- The controls will reflect the updated configuration once the browser downloads the updated implementation file

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Portal.DataAccessLayer" />