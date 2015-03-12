# Cmdlet Documentation #
Below you can find a list of all the available cmdlets. Many commands provide built-in help and examples. Retrieve the detailed help with 

```powershell
Get-Help Connect-SPOnline -Detailed
```


##
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOAuthenticationRealm](Documentation/GetSPOAuthenticationRealm.md)** |Gets the authentication realm for the current web
##
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOCustomAction](Documentation/RemoveSPOCustomAction.md)** |
**[Send&#8209;SPOMail](Documentation/SendSPOMail.md)** |
##Apps
Cmdlet|Description
:-----|:----------
**[Uninstall&#8209;SPOAppInstance](Documentation/UninstallSPOAppInstance.md)** |Removes an app from a site
**[Get&#8209;SPOAppInstance](Documentation/GetSPOAppInstance.md)** |Returns a SharePoint App Instance
**[Import&#8209;SPOAppPackage](Documentation/ImportSPOAppPackage.md)** |Adds a SharePoint App to a site
##Base Cmdlets
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOConfiguration](Documentation/SetSPOConfiguration.md)** |To be deprecated
**[Get&#8209;SPOConfiguration](Documentation/GetSPOConfiguration.md)** |To be deprecated
**[Get&#8209;SPOContext](Documentation/GetSPOContext.md)** |Returns a Client Side Object Model context
**[Get&#8209;SPOHealthScore](Documentation/GetSPOHealthScore.md)** |Retrieves the current health score value of the server
**[Disconnect&#8209;SPOnline](Documentation/DisconnectSPOnline.md)** |Disconnects the context
**[Connect&#8209;SPOnline](Documentation/ConnectSPOnline.md)** |Connects to a SharePoint site and creates an in-memory context
**[Execute&#8209;SPOQuery](Documentation/ExecuteSPOQuery.md)** |Executes any queued actions / changes on the SharePoint Client Side Object Model Context
**[Get&#8209;SPOStoredCredential](Documentation/GetSPOStoredCredential.md)** |Returns a stored credential from the Windows Credential Manager
##Branding
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOCustomAction](Documentation/GetSPOCustomAction.md)** |Returns all or a specific custom action(s)
**[Add&#8209;SPOCustomAction](Documentation/AddSPOCustomAction.md)** |Adds a custom action to a web
**[Set&#8209;SPOHomePage](Documentation/SetSPOHomePage.md)** |Sets the home page of the current web.
**[Add&#8209;SPOJavaScriptBlock](Documentation/AddSPOJavaScriptBlock.md)** |Adds a link to a JavaScript snippet/block to a web or site collection
**[Get&#8209;SPOJavaScriptLink](Documentation/GetSPOJavaScriptLink.md)** |Returns all or a specific custom action(s) with location type ScriptLink
**[Remove&#8209;SPOJavaScriptLink](Documentation/RemoveSPOJavaScriptLink.md)** |Removes a JavaScript link or block from a web or sitecollection
**[Add&#8209;SPOJavaScriptLink](Documentation/AddSPOJavaScriptLink.md)** |Adds a link to a JavaScript file to a web or sitecollection
**[Set&#8209;SPOMasterPage](Documentation/SetSPOMasterPage.md)** |Sets the default master page of the current web.
**[Add&#8209;SPONavigationNode](Documentation/AddSPONavigationNode.md)** |Adds a menu item to either the quicklaunch or top navigation
**[Remove&#8209;SPONavigationNode](Documentation/RemoveSPONavigationNode.md)** |Removes a menu item from either the quicklaunch or top navigation
**[Set&#8209;SPOTheme](Documentation/SetSPOTheme.md)** |Sets the theme of the current web.
##Content Types
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOContentType](Documentation/RemoveSPOContentType.md)** |Removes a content type
**[Get&#8209;SPOContentType](Documentation/GetSPOContentType.md)** |Retrieves a content type
**[Add&#8209;SPOContentType](Documentation/AddSPOContentType.md)** |Adds a new content type
**[Add&#8209;SPOContentTypeToList](Documentation/AddSPOContentTypeToList.md)** |Adds a new content type to a list
**[Set&#8209;SPODefaultContentTypeToList](Documentation/SetSPODefaultContentTypeToList.md)** |Sets the default content type for a list
**[Add&#8209;SPOFieldToContentType](Documentation/AddSPOFieldToContentType.md)** |Adds an existing site column to a content type
##Event Receivers
Cmdlet|Description
:-----|:----------
**[Add&#8209;SPOEventReceiver](Documentation/AddSPOEventReceiver.md)** |Adds a new event receiver
**[Get&#8209;SPOEventReceiver](Documentation/GetSPOEventReceiver.md)** |Returns all or a specific event receiver
**[Remove&#8209;SPOEventReceiver](Documentation/RemoveSPOEventReceiver.md)** |Removes/unregisters a specific event receiver
##Features
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOFeature](Documentation/GetSPOFeature.md)** |Returns all or a specific feature
**[Disable&#8209;SPOFeature](Documentation/DisableSPOFeature.md)** |Disables a feature
**[Enable&#8209;SPOFeature](Documentation/EnableSPOFeature.md)** |Enables a feature
##Fields
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOField](Documentation/RemoveSPOField.md)** |Removes a field from a list or a site
**[Get&#8209;SPOField](Documentation/GetSPOField.md)** |Returns a field from a list or site
**[Add&#8209;SPOField](Documentation/AddSPOField.md)** |Adds a field to a list or as a site column
**[Add&#8209;SPOFieldFromXml](Documentation/AddSPOFieldFromXml.md)** |Adds a field to a list or as a site column based upon a CAML/XML field definition
**[Add&#8209;SPOTaxonomyField](Documentation/AddSPOTaxonomyField.md)** |Adds a taxonomy field to a list or as a site column.
##Lists
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPODefaultColumnValues](Documentation/SetSPODefaultColumnValues.md)** |Sets default column values for a document library
**[Remove&#8209;SPOList](Documentation/RemoveSPOList.md)** |Deletes a list
**[Set&#8209;SPOList](Documentation/SetSPOList.md)** |Updates list settings
**[Get&#8209;SPOList](Documentation/GetSPOList.md)** |Returns a List object
**[New&#8209;SPOList](Documentation/NewSPOList.md)** |Creates a new list
**[Get&#8209;SPOListItem](Documentation/GetSPOListItem.md)** |Retrieves list items
**[Set&#8209;SPOListPermission](Documentation/SetSPOListPermission.md)** |Sets list permissions
**[Add&#8209;SPOView](Documentation/AddSPOView.md)** |Adds a view to a list
**[Get&#8209;SPOView](Documentation/GetSPOView.md)** |Returns one or all views from a list
**[Remove&#8209;SPOView](Documentation/RemoveSPOView.md)** |Deletes a view from a list
##Publishing
Cmdlet|Description
:-----|:----------
**[Add&#8209;SPOHtmlPublishingPageLayout](Documentation/AddSPOHtmlPublishingPageLayout.md)** |Adds a HTML based publishing page layout
**[Add&#8209;SPOPublishingPage](Documentation/AddSPOPublishingPage.md)** |Adds a publishing page
**[Add&#8209;SPOPublishingPageLayout](Documentation/AddSPOPublishingPageLayout.md)** |Adds a publishing page layout
**[Remove&#8209;SPOWikiPage](Documentation/RemoveSPOWikiPage.md)** |Removes a wiki page
**[Add&#8209;SPOWikiPage](Documentation/AddSPOWikiPage.md)** |Adds a wiki page
**[Get&#8209;SPOWikiPageContent](Documentation/GetSPOWikiPageContent.md)** |Gets the contents/source of a wiki page
**[Set&#8209;SPOWikiPageContent](Documentation/SetSPOWikiPageContent.md)** |Sets the contents of a wikipage
##Sites
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOAppSideLoading](Documentation/SetSPOAppSideLoading.md)** |Enables the App Side Loading Feature on a site
**[Get&#8209;SPOSite](Documentation/GetSPOSite.md)** |Returns the current site collection from the context.
**[Uninstall&#8209;SPOSolution](Documentation/UninstallSPOSolution.md)** |Uninstalls a sandboxed solution from a site collection
**[Install&#8209;SPOSolution](Documentation/InstallSPOSolution.md)** |Installs a sandboxed solution to a site collection
##Taxonomy
Cmdlet|Description
:-----|:----------
**[Import&#8209;SPOTaxonomy](Documentation/ImportSPOTaxonomy.md)** |Imports a taxonomy from either a string array or a file
**[Export&#8209;SPOTaxonomy](Documentation/ExportSPOTaxonomy.md)** |Exports a taxonomy to either the output or to a file.
**[Set&#8209;SPOTaxonomyFieldValue](Documentation/SetSPOTaxonomyFieldValue.md)** |Sets a taxonomy term value in a listitem field
**[Get&#8209;SPOTaxonomyItem](Documentation/GetSPOTaxonomyItem.md)** |Returns a taxonomy item
**[Get&#8209;SPOTaxonomySession](Documentation/GetSPOTaxonomySession.md)** |Returns a taxonomy session
##Tenant Administration
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOTenantSite](Documentation/SetSPOTenantSite.md)** |Office365 only: Uses the tenant API to set site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
**[Get&#8209;SPOTenantSite](Documentation/GetSPOTenantSite.md)** |Office365 only: Uses the tenant API to retrieve site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
**[Remove&#8209;SPOTenantSite](Documentation/RemoveSPOTenantSite.md)** |Office365 only: Removes a site collection from the current tenant
**[New&#8209;SPOTenantSite](Documentation/NewSPOTenantSite.md)** |Office365 only: Creates a new site collection for the current tenant
**[Get&#8209;SPOTimeZoneId](Documentation/GetSPOTimeZoneId.md)** |Returns a time zone ID
**[Get&#8209;SPOWebTemplates](Documentation/GetSPOWebTemplates.md)** |Office365 only: Returns the available web templates.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
##User and group management
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOGroup](Documentation/SetSPOGroup.md)** |Updates a group
**[New&#8209;SPOGroup](Documentation/NewSPOGroup.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Get&#8209;SPOGroup](Documentation/GetSPOGroup.md)** |Returns a specific group or all groups.
**[New&#8209;SPOUser](Documentation/NewSPOUser.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Remove&#8209;SPOUserFromGroup](Documentation/RemoveSPOUserFromGroup.md)** |Removes a user from a group
**[Add&#8209;SPOUserToGroup](Documentation/AddSPOUserToGroup.md)** |Adds a user to a group
##User Profiles
Cmdlet|Description
:-----|:----------
**[New&#8209;SPOPersonalSite](Documentation/NewSPOPersonalSite.md)** |Office365 only: Creates a personal / OneDrive For Business site
**[Get&#8209;SPOUserProfileProperty](Documentation/GetSPOUserProfileProperty.md)** |Office365 only: Uses the tenant API to retrieve site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
##Web
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOWeb](Documentation/SetSPOWeb.md)** |Sets properties on a web
##Web Parts
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOWebPart](Documentation/RemoveSPOWebPart.md)** |Removes a webpart from a page
**[Get&#8209;SPOWebPart](Documentation/GetSPOWebPart.md)** |Returns a webpart definition object
**[Get&#8209;SPOWebPartProperty](Documentation/GetSPOWebPartProperty.md)** |Returns a web part property
**[Set&#8209;SPOWebPartProperty](Documentation/SetSPOWebPartProperty.md)** |Sets a web part property
**[Add&#8209;SPOWebPartToWebPartPage](Documentation/AddSPOWebPartToWebPartPage.md)** |Adds a webpart to a web part page in a specified zone
**[Add&#8209;SPOWebPartToWikiPage](Documentation/AddSPOWebPartToWikiPage.md)** |Adds a webpart to a wiki page in a specified table row and column
##Webs
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOFile](Documentation/RemoveSPOFile.md)** |Removes a file.
**[Get&#8209;SPOFile](Documentation/GetSPOFile.md)** |Downloads a file.
**[Find&#8209;SPOFile](Documentation/FindSPOFile.md)** |Finds a file in the virtual file system of the web.
**[Add&#8209;SPOFile](Documentation/AddSPOFile.md)** |Uploads a file to Web
**[Set&#8209;SPOFileCheckedIn](Documentation/SetSPOFileCheckedIn.md)** |Checks in a file
**[Set&#8209;SPOFileCheckedOut](Documentation/SetSPOFileCheckedOut.md)** |Checks out a file
**[Add&#8209;SPOFolder](Documentation/AddSPOFolder.md)** |Creates a folder within a parent folder
**[Get&#8209;SPOHomePage](Documentation/GetSPOHomePage.md)** |Returns the URL to the home page
**[Set&#8209;SPOIndexedProperties](Documentation/SetSPOIndexedProperties.md)** |Marks values of the propertybag to be indexed by search. Notice that this will overwrite the existing flags, e.g. only the properties you define with the cmdlet will be indexed.
**[Get&#8209;SPOIndexedPropertyKeys](Documentation/GetSPOIndexedPropertyKeys.md)** |Returns the keys of the property bag values that have been marked for indexing by search
**[Get&#8209;SPOMasterPage](Documentation/GetSPOMasterPage.md)** |Returns the URLS of the default Master Page and the custom Master Page.
**[Set&#8209;SPOMinimalDownloadStrategy](Documentation/SetSPOMinimalDownloadStrategy.md)** |Activates or deactivates the minimal downloading strategy.
**[Get&#8209;SPOPropertyBag](Documentation/GetSPOPropertyBag.md)** |Returns the property bag values.
**[Remove&#8209;SPOPropertyBagValue](Documentation/RemoveSPOPropertyBagValue.md)** |Removes a value from the property bag
**[Set&#8209;SPOPropertyBagValue](Documentation/SetSPOPropertyBagValue.md)** |Sets a property bag value
**[Request&#8209;SPOReIndexWeb](Documentation/RequestSPOReIndexWeb.md)** |Marks the web for full indexing during the next incremental crawl
**[Get&#8209;SPOSubWebs](Documentation/GetSPOSubWebs.md)** |Returns the subwebs
**[Get&#8209;SPOWeb](Documentation/GetSPOWeb.md)** |Returns the current web object
**[New&#8209;SPOWeb](Documentation/NewSPOWeb.md)** |Creates a new subweb to the current web
##Workflows
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOWorkflowDefinition](Documentation/RemoveSPOWorkflowDefinition.md)** |Removes a workflow definition
**[Get&#8209;SPOWorkflowDefinition](Documentation/GetSPOWorkflowDefinition.md)** |Returns a workflow definition
**[Resume&#8209;SPOWorkflowInstance](Documentation/ResumeSPOWorkflowInstance.md)** |Resumes a previously stopped workflow instance
**[Stop&#8209;SPOWorkflowInstance](Documentation/StopSPOWorkflowInstance.md)** |Stops a workflow instance
**[Remove&#8209;SPOWorkflowSubscription](Documentation/RemoveSPOWorkflowSubscription.md)** |Removes a workflow subscription
**[Add&#8209;SPOWorkflowSubscription](Documentation/AddSPOWorkflowSubscription.md)** |Adds a workflow subscription to a list
**[Get&#8209;SPOWorkflowSubscription](Documentation/GetSPOWorkflowSubscription.md)** |Returns a workflow subscriptions from a list
