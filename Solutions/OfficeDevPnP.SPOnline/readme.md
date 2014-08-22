# OfficeDevPnP.SPOnline PowerShell Commands #

### Summary ###
This solution shows how you can build a library of PowerShell commands that act towards SharePoint Online. The commands use CSOM and can work against both SharePoint Online as SharePoint On-Premises.

***Notice***. *We are in progress of updating these commands to use the `[Core component](https://github.com/OfficeDev/PnP/tree/master/OfficeDevPnP.Core)`, so that code is shared cross provider hosted platform and the PowerShell commands for proper code. *reuse.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

*Name indicates that this would only work with SP online, but you can change the connection style to support on-premises as well.*

### Prerequisites ###
Any special pre-requisites?

### Solution ###
Solution | Author(s)
---------|----------
OfficeDevPnP.SPOnline | Erwin van Hunen (Knowit Reaktor Stockholm AB)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 18th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW TO USE AGAINST OFFICE 365 #
A build script will copy the required files to a folder in your users folder, called:
*C:\Users\<YourUserName>\Documents\WindowsPowerShell\Modules\OfficeDevPnP.SPOnline.Commands*

This will automatically load the module after starting PowerShell 3.0.
To use the library you first need to connect to your tenant:

	Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials (Get-Credential)

In case of an unattended script you might want to add a new entry in your credential manager of windows. 

![](http://i.imgur.com/6NiMaFL.png)
 
Select Windows Credentials and add a new credential:

![](http://i.imgur.com/rhtgL1U.png)
 
Now you can use this entry to connect to your tenant as follows:

	Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials yourlabel


## Commands ##
Here's list of different provider commands. We are looking to provide more examples to usage of these sooner or later.

Command | Description
--------|------------
**Add-SPOApp** | Adds an app, uploads a local .app file to a site
**Add-SPOContentType** | Creates a new content type
**Add-SPOCustomAction** | Adds a custom action to a site
**Add-SPOField** | Adds a new field
**Add-SPOFieldToContentType** | Adds a field to an existing content type
**Add-SPOFile** | Uploads a file to a site
**Add-SPONavigationLink** | Adds a new link to the quicklaunch navigation
**Add-SPOTaxonomyField** | Creates a new Taxonomy field
**Add-SPOUserToGroup** | Adds a user to a group
**Add-SPOView** | Adds a new view to an existing list
**Add-SPOWebPart** | Adds a webpart to an existing page. The webparts needs to be available on the server
**Add-SPOWikiPage** | Adds a new wikipages to a site
**Connect-SPOnline** | Creates a new connection context to be used by all commands
**Disable-SPOFeature** | Disables a feature
**Disconnect-SPOnline** | Disconnects the context
**Enable-SPOFeature**| Enables a feature
**Execute-SPOQuery** | Convenience command that executes the current query. See **Get-SPOContext**
**Export-SPOTaxonomy** | Exports (a part of) the taxonomy terms
**Find-SPOFile** | Finds a file in the current site
**Get-SPOAppInstance** | Gets an app instance
**Get-SPOConfiguration** |	Gets the current configuration (currently not being used)
**Get-SPOContentType** | Returns a content type
**Get-SPOContext** | Returns a ClientContext object for use in more detailed powershell commands. E.g.

	Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials CREDS
	$ctx = Get-SPOContext
	$list = $ctx.Web.Lists.GetByTitle(“Test”)
	$ctx.ExecuteQuery()
	(optionally you can use execute-spoquery instead of $ctx.ExecuteQuery())

**Get-SPOCustomAction**                                                                                                    	Returns an existing custom action

**Get-SPOEventReceiver**                                                                                                   	Returns event receivers

**Get-SPOFeature**                                                                                                        	Returns features

**Get-SPOField**                                                                                                           	Returns a field

**Get-SPOFile**                                                                                                            	Returns a file

**Get-SPOGroup**                                                                                                           	Returns a group

**Get-SPOHealthScore**                                                                                                    	Gets the current health score of the server

**Get-SPOHomePage**                                                                                                        	Returns the url of the current homepage

**Get-SPOList**                                                                                                            	Returns a list

**Get-SPOMasterPage**                                                                                                     	Returns the urls of the current assigned masterpages

**Get-SPOPropertyBag**                                                                                                     	Returns the propertybag

**Get-SPOSite**                                                                                                            	Returns the current site

**Get-SPOStoredCredential**                                                                                                	Returns a stored credentials from the credential manager as a PowerShell credential

**Get-SPOSubWebs**                                                                                                        	Returns the subwebs

**Get-SPOTaxonomyItem**                                                                                                   	Returns a specific item from the taxonomy

**Get-SPOTaxonomySession**                                                                                                 	Returns a taxonomy session

**Get-SPOTenantSite** Returns a site from your tenant administration. For this to work you need to connect to your tenant admin first with 

	Connect-SPOnline –Url https://yoursite-admin.sharepoint.com 

**Get-SPOTimeZoneId**                                                                                                     	Returns all timezone ids to  be used to create a new site collection in your tenant

**Get-SPOView**                                                                                                            	Returns the views of a list

**Get-SPOWeb**                                                                                                             	Returns the current web

**Get-SPOWebPart**                                                                                                         	Returns the webparts on a given page

**Get-SPOWebTemplates**                                                                                                   	Returns all webtemplates
For this to work you need to connect to your tenant admin first with 

	Connect-SPOnline –Url https://yoursite-admin.sharepoint.com

**Get-SPOWikiPageContent**                                                                                                	Returns the HTML content of a wikipage

**Import-SPOTaxonomy**                                                                                                     	Imports a taxonomy into the managed metadata service. See the help of the command for examples

**New-SPOList**                                                                                                            	Creates a new list

**New-SPOOnPremSite**                                                                                                      	Currently not implemented

**New-SPOTenantSite**                                                                                                      	Creates a new site collection in your tenant
For this to work you need to connect to your tenant admin first with 

	Connect-SPOnline –Url https://yoursite-admin.sharepoint.com

**New-SPOUser**                                                                                                            	Equivalent to web.EnsureUser(user)

**New-SPOWeb**                                                                                                            	Creates a new web

**Register-SPOEventReceiver**                                                                                             	Registers an event receiver to a site/list

**Remove-SPOApp**                                                                                                          	Removes an app from the site contents

**Remove-SPOContentType**                                                                                                 	Removes a content types

**Remove-SPOCustomAction**                                                                                                 	Removes a custom action from a site

**Remove-SPOEventReceiver**                                                                                                	Removes an event receiver

**Remove-SPOField **                                                                                                       	Removes a field

**Remove-SPOList**                                                                                                         	Removes a list

**Remove-SPOPropertyBagValue**                                                                                             	Removes a property bag entry

**Remove-SPOTenantSite**                                                                                                   	Removes a site from your tenant
For this to work you need to connect to your tenant admin first with 

	Connect-SPOnline –Url https://yoursite-admin.sharepoint.com

**Remove-SPOUserFromGroup**                                                                                                	Removes a user from a group

**Remove-SPOView**                                                                                                        	Removes a view

**Remove-SPOWebPart**                                                                                                      	Removes a webpart

**Remove-SPOWikiPage**                                                                                                     	Removes a wiki page

**Request-SPOReIndexWeb**                                                                                                  	Requests a site to fully crawled the next incremental crawl

**Set-SPOAppSideLoading**                                                                                                  	Turns app sideloading on or off for a site

**Set-SPOConfiguration**                                                                                                  	Sets a configuration value, currently not used

**Set-SPOFileCheckedIn**                                                                                                   	Checks in a file

**Set-SPOFileCheckedOut**                                                                                                  	Checks out a file

**Set-SPOHomePage**                                                                                                        	Sets the current homepage

**Set-SPOIndexedProperties**                                                                                               	Sets what property of the propertybag should be indexed by search

**Set-SPOMasterPage**                                                                                                      	Sets the masterpage

**Set-SPOMinimalDownloadStrategy**                                                                                         	Turns MDS on or off

**Set-SPOPropertyBagValue**                                                                                                	Sets a property bag value

**Set-SPOTaxonomyFieldValue**                                                                                              	Sets a taxonomy field value

**Set-SPOTheme**                                                                                                           	Sets the current theme

**Set-SPOWebPartProperty**                                                                                                 	Sets a webpart property

**Set-SPOWikiPageContent**                                                                                                 	Sets the content of a wikipage
