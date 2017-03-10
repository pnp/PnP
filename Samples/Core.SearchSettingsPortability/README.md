# SEARCH IMPORT/EXPORT #

### Summary ###
This sample demonstrates how to import and export operations on search settings via a CSOM console application.

### Applies to ###
-  Office 365 Multi-Tenant (MT)


### Solution ###
Solution | Author(s)
---------|----------
Core.SearchSettingsPortability | Brian Michely (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 30, 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# GENERAL COMMENTS #
"Search Portability" supports transferring the following items:
- Results sources
- Query rules
- Result types
- Schema
- Custom ranking models

You can transfer settings between a tenant, site collections or sites. This can be done via the SharePoint UI, or via CSOM, which is the basis for this sample. This sample is a simple console application that prompts for the following arguments:
- Type of operation to perform (Import | Export)
- Path for settings file (where to save for export, or where to import from)
- Url of SharePoint Online site
- User name (ex: yourname@yourtenant.microsoftonline.com)
- Password

The code will attempt to authenticate via the SharePointOnlineCredentials object which provides credentials to access SharePoint Online resources

```C#
context.Credentials = new SharePointOnlineCredentials(userName, password);
context.Load(context.Web, w => w.Title);
context.ExecuteQuery();
```

Once authenticated, the add-in will use CSOM to either Import or Export by calling the appropriate method. The methods use the [SearchConfigurationPortability](http://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.search.portability.searchconfigurationportability(v=office.15).aspx) and [SeachObjectOwner](http://msdn.microsoft.com/en-us/library/office/microsoft.office.server.search.administration.searchobjectowner(v=office.15).aspx) objects and the code executes either the **ExportSearchSettings** method or **ImportSearchSettings** method to perform the action requested by the user in the console.

![ExportSearchSettings method code](http://i.imgur.com/UgjWZi2.png)

![ImportSearchSettings method code](http://i.imgur.com/KguZrmy.png)

Example of the Import method
The end result from an export is a search settings .xml file that can be use elsewhere by performing the import shown above.


<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.SearchSettingsPortability" />