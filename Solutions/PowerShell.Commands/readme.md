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
**[Connect-SPOnline](Documentation/ConnectSPOnline.md)** | Creates a new connection context to be used by all commands
**[Disconnect-SPOnline](Documentation/DisconnectSPOnline.md)** | Disconnects the context
**[Execute-SPOQuery](Documentation/ExecuteSPOQuery.md)** | Convenience command that executes the current query. See **Get-SPOContext**
**[Get-SPOConfiguration](Documentation/GetSPOConfiguration.md)** |	Gets the current configuration, alike a local property bag
**[Get-SPOContext](Documentation/GetSPOContext.md)** | Returns a ClientContext object for use in your own powershell scripts that use CSOM
**[Get-SPOHealthScore](Documentation/GetSPOHealthScore.md)** | Gets the current health score of the server
**[Get-SPOStoredCredential](Documentation/GetSPOStoredCredential.md)** | Returns a stored credentials from the credential manager as a PowerShell credential
**[Set-SPOConfiguration](Documentation/SetSPOConfiguration.md)** | Sets a configuration value, stored locally, alike a local property bag

#### Tenant Administration Cmdlets ####
Command | Description
:--------|:------------
**[Get-SPOTenantSite](Documentation/GetSPOTenantSite.md)** | Returns a site from your tenant administration. For this to work you need to connect to your tenant admin first with 
**[Get-SPOTimeZoneId](Documentation/GetSPOTimeZoneId.md)** |	Returns all timezone ids to  be used to create a new site collection in your tenant
**[Get-SPOWebTemplates](Documentation/GetSPOWebTemplates.md)** | Returns all webtemplates
**[New-SPOTenantSite](Documentation/NewSPOTenantSite.md)** | Creates a new site collection in your tenant
**[Remove-SPOTenantSite](Documentation/RemoveSPOTenantSite.md)** | Removes a site from your tenant
**[Set-SPOTenantSite](Documentation/SetSPOTenantSite.md)** | Sets properties on an existing tenant site collection

#### App Cmdlets ####
Command | Description
:--------|:------------
**[Import-SPOAppPackage](Documentation/ImportSPOAppPackage.md)** | Uploads an app package to a site
**[Get-SPOAppInstance](Documentation/GetSPOAppInstance.md)** | Gets an app instance
**[Uninstall-SPOAppInstance](Documentation/UninstallSPOAppInstance.md)** | Removes an app instance from a site

#### App Authentication Cmdlets ####
Command | Description
:--------|:------------
**[Get-SPOAuthenticationRealm](Documentation/GetSPOAuthenticationRealm.md)** | Returns the authentication realm for use for app only authentication. See **[Connect-SPOnline](Documentation/ConnectSPOnline.md) -AppId -AppSecret -Realm**

#### Branding Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOCustomAction](Documentation/AddSPOCustomAction.md)** | Adds a custom action to a site
**[AddJavaScriptBlock](Documentation/AddSPOJavaScriptBlock.md)** | Adds a JavaScript snippet to a web or site as a custom action
**[AddJavaScriptLink](Documentation/AddSPOJavaScriptLink.md)** | Adds a link to a JavaScript file to a web or site as a custom action
**[Get-SPOCustomAction](Documentation/GetSPOCustomAction.md)** | Returns an existing custom action
**[Get-SPOJavasScriptLink](Documentation/GetSPOJavaScriptLink.md)** | Returns all javascript snippets and links
**[Remove-SPOCustomAction](Documentation/RemoveSPOCustomAction.md)** | Removes a custom action from a web
**[Add-SPONavigationNode](Documentation/AddSPONavigationNode.md)** | Adds a new link to the quicklaunch or top navigation
**[Remove-SPONavigationNode](Documentation/RemoveSPONavigationNode.md)** | Removes a link from the quicklaunch or top navigation
**[Set-SPOHomePage](Documentation/SetSPOHomepage.md)** | Sets the current homepage
**[Set-SPOMasterPage](Documentation/SetSPOMasterPage.md)** | Sets the masterpage
**[Set-SPOTheme](Documentation/SetSPOTheme.md)** | Sets the current theme

#### Content Type Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOContentType](Documentation/AddSPOContentType.md)** | Creates a new content type
**[Add-SPOContentTypeToList](Documentation/AddSPOContentTypeToList.md)** | Adds an existing content type to a list
**[Add-SPOFieldToContentType](Documentation/AddSPOFieldToContentType.md)** | Adds a field to an existing content type
**[Get-SPOContentType](Documentation/GetSPOContentType.md)** | Returns a content type
**[Remove-SPOContentType](Documentation/RemoveSPOContentType.md)** | Removes a content types
**[Set-SPODefaultContentTypeToList](Documentation/SetSPODefaultContentTypeToList)** | Sets the default content type to use for a list

#### Event Receiver Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOEventReceiver](Documentation/AddSPOEventReceiver.md)** |	Registers an event receiver to a site/list
**[Get-SPOEventReceiver](Documentation/GetSPOEventReceiver.md)** | Returns event receivers
**[Remove-SPOEventReceiver](Documentation/RemoveSPOEventReceiver.md)** | Removes an event receiver

#### Feature Cmdlets ####
Command | Description
:--------|:------------
**[Disable-SPOFeature](Documentation/DisableSPOFeature.md)** | Disables a feature
**[Enable-SPOFeature](Documentation/EnableSPOFeature.md)**| Enables a feature
**[Get-SPOFeature](Documentation/GetSPOFeature.md)** | Returns features

#### Field Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOField](Documentation/AddSPOField.md)** | Adds a new field
**[Add-SPOFieldFromXml](Documentation/AddSPOFieldFromXml.md)** | Adds a new field based on a CAML xml snippet, see http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx
**[Add-SPOTaxonomyField](Documentation/AddSPOTaxonomyField.md)** | Creates a new Taxonomy field
**[Get-SPOField](Documentation/GetSPOField.md)** | Returns a field
**[Remove-SPOField](Documentation/RemoveSPOField.md)** | Removes a field

#### List Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOView](Documentation/AddSPOView.md)** | Adds a new view to an existing list
**[Get-SPOList](Documentation/GetSPOList.md)** | Returns a list
**[Get-SPOView](Documentation/GetSPOView.md)** | Returns the views of a list
**[Get-SPOListItem](Documentation/GetSPOListItem.md)** | Retrieve list items by id, unique id, or CAML. Optionally you can define which fields to load.
**[New-SPOList](Documentation/NewSPOList.md)** | Creates a new list
**[Remove-SPOList](Documentation/RemoveSPOList.md)** | Removes a list
**[Remove-SPOView](Documentation/RemoveSPOView.md)** | Removes a view
**[Set-SPODefaultColumnValues](Documentation/SetSPODefaultColumnValues.md)** | Sets default column values for a document library
**[Set-SPOList](Documentation/SetSPOList.md)** | Sets list properties
**[Set-SPOListPermission](Documentation/SetSPOListPermission.md)** | Sets list permissions

#### User and Group Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOUserToGroup](Documentation/AddSPOUserToGroup.md)** | Adds a user to a group
**[Get-SPOGroup](Documentation/GetSPOGroup.md)** | Returns a group
**[New-SPOGroup](Documentation/NewSPOGroup.md)** | Creates a group
**[New-SPOUser](Documentation/NewSPOUser.md)** | Adds a user to the Site User Info List. Equivalent to web.EnsureUser(user)
**[Remove-SPOUserFromGroup](Documentation/RemoveSPOUserFromGroup.md)** | Removes a user from a group
**[Set-SPOGroup](Documentation/SetSPOGroup.md)** | Sets a group as an associated group (Owners, Members, Visitors) or adds or removes a role assignment (e.g. "Contribute", "Read", etc.)

#### Site Cmdlets ####
Command | Description
:--------|:------------
**[Get-SPOSite](Documentation/GetSPOSite.md)** | Returns the current site
**[Set-SPOAppSideLoading](Documentation/SetSPOAppSideLoading.md)** | Turns app sideloading on or off for a site

#### Taxonomy / Managed Metadata Cmdlets ####
Command | Description
:--------|:------------
**[Export-SPOTaxonomy](Documentation/ExportSPOTaxonomy.md)** | Exports (a part of) the taxonomy terms
**[Get-SPOTaxonomyItem](Documentation/GetSPOTaxonomyItem.md)** | Returns a specific item from the taxonomy
**[Get-SPOTaxonomySession](Documentation/GetSPOTaxonomySession.md)** | Returns a taxonomy session
**[Import-SPOTaxonomy](Documentation/ImportSPOTaxonomy.md)** | Imports a taxonomy into the managed metadata service. See the help of the command for examples
**[Set-SPOTaxonomyFieldValue](Documentation/SetSPOTaxonomyFieldValue.md)** | Sets a taxonomy field value

#### Utility Cmdlets ####
Command | Description
:--------|:------------
**[Send-SPOMail](Documentation/SendSPOMail.md)** | Sends an email. Server defaults to smtp.office365.com but can be changed.

#### Web Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOFile](Documentation/AddSPOFile.md)** | Uploads a file to a site
**[Add-SPOFolder](Documentation/AddSPOFolder.md)** | Creates a folder in a site
**[Find-SPOFile](Documentation/FindSPOFile.md)** | Finds a file in the current site
**[Get-SPOFile](Documentation/GetSPOFile.md)** | Returns a file
**[Get-SPOHomePage](Documentation/GetSPOHomePage.md)** | Returns the url of the current homepage
**[Get-SPOMasterPage](Documentation/GetSPOMasterPage.md)** | Returns the urls of the current assigned masterpages
**[Get-SPOPropertyBag](Documentation/GetSPOPropertyBag.md)** | Returns the propertybag
**[Get-SPOSubWebs](Documentation/GetSPOSubwebs.md)** | Returns the subwebs
**[Get-SPOWeb](Documentation/GetSPOWeb.md)** | Returns the current web
**[Get-SPOIndexedPropertyKeys](Documentation/GetSPOIndexedPropertyKeys.md)** | Returns all keys in the property bag set for indexing
**[New-SPOWeb](Documentation/NewSPOWeb.md)** | Creates a new web
**[Remove-SPOPropertyBagValue](Documentation/RemoveSPOPropertyBagValue.md)** | Removes a property bag entry
**[Remove-SPOFile](Documentation/RemoveSPOFile.md)** | Removes a file
**[Request-SPOReIndexWeb](Documentation/RequestSPOReIndexWeb)** |	Requests a site to fully crawled the next incremental crawl
**[Set-SPOFileCheckedIn](Documentation/SetSPOFileCheckedIn.md)** | Checks in a file
**[Set-SPOFileCheckedOut](Documentation/SetSPOFileCheckedOut.md)** | Checks out a file
**[Set-SPOIndexedProperties](Documentation/SetSPOIndexedProperties.md)** |Sets what property of the propertybag should be indexed by search
**[Set-SPOMinimalDownloadStrategy](Documentation/SetSPOMinimalDownloadStrategy.md)** | Turns MDS on or off
**[Set-SPOPropertyBagValue](Documentation/SetSPOPropertyBagValue.md)** | Sets a property bag value

#### Web Part Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOWebPartToWebPartPage](Documentation/AddSPOWebPartToWebPartPage.md)** | Adds a webpart to an existing web part page. The webparts needs to be available on the server
**[Add-SPOWebPartToWikiPage](Documentation/AddSPOWebPartToWikiPage.md)** | Adds a webpart to an existing wiki page. The webparts needs to be available on the server
**[Get-SPOWebPart](Documentation/GetSPOWebPart.md)** | Returns the webparts on a given page
**[Remove-SPOWebPart](Documentation/GetSPOWebPart.md)** |	Removes a webpart from a page
**[Set-SPOWebPartProperty](Documentation/SetSPOWebPartProperty.md)** | Sets a webpart property
**[Get-SPOWebPartProperty](Documentation/GetSPOWebPartProperty.md)** | Returns webpart properties

#### Wiki Page Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOWikiPage](Documentation/AddSPOWikiPage.md)** | Adds a new wikipages to a site
**[Get-SPOWikiPageContent](Documentation/GetSPOWikiPageContent.md)** | Returns the HTML content of a wikipage
**[Remove-SPOWikiPage](Documentation/RemoveSPOWikiPage.md)**| Removes a wiki page
**[Set-SPOWikiPageContent](Documentation/SetSPOWikiPageContent.md)** | Sets the content of a wikipage

#### Publishing Page Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOPublishingPage](Documentation/AddSPOPublishingPage.md)** | Adds a new publishing page to a site
**[Add-SPOPublishingPageLayout](Documentation/AddSPOPublishingPageLayout.md)** | Adds a new .aspx publishing page layout to a site
**[Add-SPOHtmlPublishingPageLayout](Documentation/AddSPOHtmlPublishingPageLayout.md)** | Adds a new .html publishing page layout to a site

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
**[Get-SPOUserProfileProperty](Documentation/GetSPOUserProfileProperty.md)** | Returns the user profile properties for one or more users
**[New-SPOPersonalSite](Documentation/NewSPOPersonalSite.md)** | Provisions a profile site. Only works towards Office365

#### Workflow Cmdlets ####
Command | Description
:--------|:------------
**[Add-SPOWorkflowSubscription](Documentation/AddSPOWorkflowSubscription.md)** | Adds a new subscription (association) to a list or web
**[Get-SPOWorkflowDefinition](Documentation/GetSPOWorkflowDefinition.md)** | Returns all or a specific workflow definition (reusable workflow)
**[Get-SPOWorkflowSubscription](Documentation/GetSPOWorkflowSubscription.md)** | Returns all or a specific workflow subscription
**[Remove-SPOWorkflowDefinition](Documentation/RemoveSPOWorkflowDefinition.md)** | Removes a workflow definition (reusable workflow)
**[Remove-SPOWorkflowSubscription](Documentation/RemoveSPOWorkflowSubscription.md)** | Removes a workflow subscription
**[Resume-SPOWorkflowInstance](Documentation/ResumeSPOWorkflowInstance.md)** | Resumes a workflow instance
**[Stop-SPOWorkflowInstance](Documentation/StopSPOWorkflowInstance.md)** | Stops (cancels) a workflow instance
