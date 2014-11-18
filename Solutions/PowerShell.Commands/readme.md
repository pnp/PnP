# OfficeDevPnP.PowerShell Commands #

### Summary ###
This solution shows how you can build a library of PowerShell commands that act towards SharePoint Online. The commands use CSOM and can work against both SharePoint Online as SharePoint On-Premises.

### Applies to ###
-  Office 365 Multi Tenant (MT)
-  Office 365 Dedicated (D)
-  SharePoint 2013 on-premises

### Prerequisites ###
In order to build the setup project the Wix toolset needs to be installed. You can obtain this from http://wix.codeplex.com.

### Solution ###
Solution | Author(s)
---------|----------
OfficeDevPnP.PowerShell | Erwin van Hunen (**Knowit Reaktor Stockholm AB**)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | August 18th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# HOW TO USE AGAINST OFFICE 365 #
A build script will copy the required files to a folder in your users folder, called:
*C:\Users\<YourUserName>\Documents\WindowsPowerShell\Modules\OfficeDevPnP.PowerShell.Commands*

This will automatically load the module after starting PowerShell 3.0.
To use the library you first need to connect to your tenant:

```powershell
Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials (Get-Credential)
```

In case of an unattended script you might want to add a new entry in your credential manager of windows. 

![](http://i.imgur.com/6NiMaFL.png)
 
Select Windows Credentials and add a new *generic* credential:

![](http://i.imgur.com/rhtgL1U.png)
 
Now you can use this entry to connect to your tenant as follows:

```powershell
Connect-SPOnline –Url https://yoursite.sharepoint.com –Credentials yourlabel
```

## Commands ##
Here's a list of different provider commands. Many commands provide built-in help and examples, e.g. 

```powershell
Get-Help Connect-SPOnline -Detailed
```

We are looking to provide more examples to usage of these sooner or later.


#### Base Cmdlets ####
Command | Description
:--------|:------------
**Connect-SPOnline** | Creates a new connection context to be used by all commands
**Disconnect-SPOnline** | Disconnects the context
**Execute-SPOQuery** | Convenience command that executes the current query. See **Get-SPOContext**
**Get-SPOConfiguration** |	Gets the current configuration, alike a local property bag
**Get-SPOContext** | Returns a ClientContext object for use in your own powershell scripts that use CSOM
**Get-SPOHealthScore** | Gets the current health score of the server
**Get-SPOStoredCredential** | Returns a stored credentials from the credential manager as a PowerShell credential
**Set-SPOConfiguration** | Sets a configuration value, stored locally, alike a local property bag

#### Tenant Administration Cmdlets ####
Command | Description
:--------|:------------
**Get-SPOTenantSite** | Returns a site from your tenant administration. For this to work you need to connect to your tenant admin first with 
**Get-SPOTimeZoneId** |	Returns all timezone ids to  be used to create a new site collection in your tenant
**Get-SPOWebTemplates** | Returns all webtemplates
**New-SPOTenantSite** | Creates a new site collection in your tenant
**Remove-SPOTenantSite** | Removes a site from your tenant
**Set-SPOTenantSite** | Sets properties on an existing tenant site collection

#### App Cmdlets ####
Command | Description
:--------|:------------
**Import-SPOAppPackage** | Uploads an app package to a site
**Get-SPOAppInstance** | Gets an app instance
**Uninstall-SPOAppInstance** | Removes an app instance from a site

#### App Authentication Cmdlets ####
Command | Description
:--------|:------------
**Get-SPOAuthenticationRealm** | Returns the authentication realm for use for app only authentication. See **Connect-SPOnline -AppId -AppSecret -Realm**

#### Content Type Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOContentType** | Creates a new content type
**Add-SPOContentTypeToList** | Adds an existing content type to a list
**Add-SPOFieldToContentType** | Adds a field to an existing content type
**Get-SPOContentType** | Returns a content type
**Remove-SPOContentType** | Removes a content types
**Set-SPODefaultContentTypeToList** | Sets the default content type to use for a list

#### Event Receiver Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOEventReceiver** |	Registers an event receiver to a site/list
**Get-SPOEventReceiver** | Returns event receivers
**Remove-SPOEventReceiver** | Removes an event receiver

#### Feature Cmdlets ####
Command | Description
:--------|:------------
**Disable-SPOFeature** | Disables a feature
**Enable-SPOFeature**| Enables a feature
**Get-SPOFeature** | Returns features

#### Field Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOField** | Adds a new field
**Add-SPOFieldFromXml** | Adds a new field based on a CAML xml snippet, see http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx
**Add-SPOTaxonomyField** | Creates a new Taxonomy field
**Get-SPOField** | Returns a field
**Remove-SPOField** | Removes a field

#### List Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOView** | Adds a new view to an existing list
**Get-SPOList** | Returns a list
**Get-SPOView** | Returns the views of a list
**New-SPOList** | Creates a new list
**Remove-SPOList** | Removes a list
**Remove-SPOView** | Removes a view
**Set-SPODefaultColumnValues** | Sets default column values for a document library

#### User and Group Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOUserToGroup** | Adds a user to a group
**Get-SPOGroup** | Returns a group
**New-SPOGroup** | Creates a group
**New-SPOUser** | Adds a user to the Site User Info List. Equivalent to web.EnsureUser(user)
**Remove-SPOUserFromGroup** | Removes a user from a group

#### Site Cmdlets ####
Command | Description
:--------|:------------
**Get-SPOSite** | Returns the current site
**Set-SPOAppSideLoading** | Turns app sideloading on or off for a site

#### Taxonomy / Managed Metadata Cmdlets ####
Command | Description
:--------|:------------
**Export-SPOTaxonomy** | Exports (a part of) the taxonomy terms
**Get-SPOTaxonomyItem** | Returns a specific item from the taxonomy
**Get-SPOTaxonomySession** | Returns a taxonomy session
**Import-SPOTaxonomy** | Imports a taxonomy into the managed metadata service. See the help of the command for examples
**Set-SPOTaxonomyFieldValue** | Sets a taxonomy field value

#### Utility Cmdlets ####
Command | Description
:--------|:------------
**Send-SPOMail** | Sends an email. Server defaults to smtp.office365.com but can be changed.

#### Web Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOCustomAction** | Adds a custom action to a site
**Add-SPOFile** | Uploads a file to a site
**Add-SPOFolder** | Creates a folder in a site
**Add-SPONavigationNode** | Adds a new link to the quicklaunch or top navigation
**Find-SPOFile** | Finds a file in the current site
**Get-SPOCustomAction** | Returns an existing custom action
**Get-SPOFile** | Returns a file
**Get-SPOHomePage** | Returns the url of the current homepage
**Get-SPOMasterPage** | Returns the urls of the current assigned masterpages
**Get-SPOPropertyBag** | Returns the propertybag
**Get-SPOSubWebs** | Returns the subwebs
**Get-SPOWeb** | Returns the current web
**Get-SPOIndexedPropertyKeys** | Returns all keys in the property bag set for indexing
**New-SPOWeb** | Creates a new web
**Remove-SPOCustomAction** | Removes a custom action from a web
**Remove-SPONavigationNode** | Removes a link from the quicklaunch or top navigation
**Remove-SPOPropertyBagValue** | Removes a property bag entry
**Request-SPOReIndexWeb** |	Requests a site to fully crawled the next incremental crawl
**Set-SPOFileCheckedIn** | Checks in a file
**Set-SPOFileCheckedOut** | Checks out a file
**Set-SPOHomePage** | Sets the current homepage
**Set-SPOIndexedProperties** |Sets what property of the propertybag should be indexed by search
**Set-SPOMasterPage** | Sets the masterpage
**Set-SPOMinimalDownloadStrategy** | Turns MDS on or off
**Set-SPOPropertyBagValue** | Sets a property bag value
**Set-SPOTheme** | Sets the current theme

#### Web Part Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOWebPartToWebPartPage** | Adds a webpart to an existing web part page. The webparts needs to be available on the server
**Add-SPOWebPartToWikiPage** | Adds a webpart to an existing wiki page. The webparts needs to be available on the server
**Get-SPOWebPart** | Returns the webparts on a given page
**Remove-SPOWebPart** |	Removes a webpart from a page
**Set-SPOWebPartProperty** | Sets a webpart property

#### Wiki Page Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOWikiPage** | Adds a new wikipages to a site
**Get-SPOWikiPageContent** | Returns the HTML content of a wikipage
**Remove-SPOWikiPage**| Removes a wiki page
**Set-SPOWikiPageContent** | Sets the content of a wikipage

#### Publishing Page Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOPublishingPage** | Adds a new publishing page to a site
**Add-SPOPublishingPageLayout** | Adds a new .aspx publishing page layout to a site
**Add-SPOHtmlPublishingPageLayout** | Adds a new .html publishing page layout to a site

Examples (Note: The associated content type in the example is the "Welcome Page" built in content type)

```powershell
Add-SPOPublishingPage -PageName "your-page-name" -PageTemplateName "BlankWebPartPage" -Title "Your Page Title" -Publish

Add-SPOPublishingPageLayout -SourceFilePath "Path-To-Your-Page-Layout" -Title "Your Title" -Description "Your Description" -AssociatedContentTypeID "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB06458
4E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4"

Add-SPOHtmlPublishingPageLayout -SourceFilePath "Path-To-Your-Page-Layout" -Title "Your Title" -Description "Your Description" -AssociatedContentTypeID "0x010100C568DB52D9D0A14D9B2FDCC96666E9F2007948130EC3DB06458
4E219954237AF390064DEA0F50FC8C147B0B6EA0636C4A7D4"
```

#### User Profile / OD4B Cmdlets ####
Command | Description
:--------|:------------
**Get-SPOUserProfileProperty** | Returns the user profile properties for one or more users
**New-SPOPersonalSite** | Provisions a profile site. Only works towards Office365

#### Workflow Cmdlets ####
Command | Description
:--------|:------------
**Add-SPOWorkflowSubscription** | Adds a new subscription (association) to a list or web
**Get-SPOWorkflowDefinition** | Returns all or a specific workflow definition (reusable workflow)
**Get-SPOWorkflowSubscription** | Returns all or a specific workflow subscription
**Remove-SPOWorkflowDefinition** | Removes a workflow definition (reusable workflow)
**Remove-SPOWorkflowSubscription** | Removes a workflow subscription
**Resume-SPOWorkflowInstance** | Resumes a workflow instance
**Stop-SPOWorkflowInstance** | Stops (cancels) a workflow instance
