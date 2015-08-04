# Cmdlet Documentation #
Below you can find a list of all the available cmdlets. Many commands provide built-in help and examples. Retrieve the detailed help with 

```powershell
Get-Help Connect-SPOnline -Detailed
```

##Apps
Cmdlet|Description
:-----|:----------
**[Uninstall&#8209;SPOAppInstance](UninstallSPOAppInstance.md)** |Removes an app from a site
**[Get&#8209;SPOAppInstance](GetSPOAppInstance.md)** |Returns a SharePoint AddIn Instance
**[Import&#8209;SPOAppPackage](ImportSPOAppPackage.md)** |Adds a SharePoint Addin to a site
##Base Cmdlets
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOAuthenticationRealm](GetSPOAuthenticationRealm.md)** |Gets the authentication realm for the current web
**[Get&#8209;SPOAzureADManifestKeyCredentials](GetSPOAzureADManifestKeyCredentials.md)** |Creates the JSON snippet that is required for the manifest json file for Azure WebApplication / WebAPI apps
**[Set&#8209;SPOConfiguration](SetSPOConfiguration.md)** |To be deprecated
**[Get&#8209;SPOConfiguration](GetSPOConfiguration.md)** |To be deprecated
**[Get&#8209;SPOContext](GetSPOContext.md)** |Returns a Client Side Object Model context
**[Get&#8209;SPOHealthScore](GetSPOHealthScore.md)** |Retrieves the current health score value of the server
**[Disconnect&#8209;SPOnline](DisconnectSPOnline.md)** |Disconnects the context
**[Connect&#8209;SPOnline](ConnectSPOnline.md)** |Connects to a SharePoint site and creates an in-memory context
**[Execute&#8209;SPOQuery](ExecuteSPOQuery.md)** |Executes any queued actions / changes on the SharePoint Client Side Object Model Context
**[Get&#8209;SPOStoredCredential](GetSPOStoredCredential.md)** |Returns a stored credential from the Windows Credential Manager
##Branding
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOCustomAction](RemoveSPOCustomAction.md)** |Removes a custom action
**[Get&#8209;SPOCustomAction](GetSPOCustomAction.md)** |Returns all or a specific custom action(s)
**[Add&#8209;SPOCustomAction](AddSPOCustomAction.md)** |Adds a custom action to a web
**[Set&#8209;SPOHomePage](SetSPOHomePage.md)** |Sets the home page of the current web.
**[Add&#8209;SPOJavaScriptBlock](AddSPOJavaScriptBlock.md)** |Adds a link to a JavaScript snippet/block to a web or site collection
**[Get&#8209;SPOJavaScriptLink](GetSPOJavaScriptLink.md)** |Returns all or a specific custom action(s) with location type ScriptLink
**[Remove&#8209;SPOJavaScriptLink](RemoveSPOJavaScriptLink.md)** |Removes a JavaScript link or block from a web or sitecollection
**[Add&#8209;SPOJavaScriptLink](AddSPOJavaScriptLink.md)** |Adds a link to a JavaScript file to a web or sitecollection
**[Set&#8209;SPOMasterPage](SetSPOMasterPage.md)** |Sets the default master page of the current web.
**[Set&#8209;SPOMinimalDownloadStrategy](SetSPOMinimalDownloadStrategy.md)** |Activates or deactivates the minimal downloading strategy.
**[Add&#8209;SPONavigationNode](AddSPONavigationNode.md)** |Adds a menu item to either the quicklaunch or top navigation
**[Remove&#8209;SPONavigationNode](RemoveSPONavigationNode.md)** |Removes a menu item from either the quicklaunch or top navigation
**[Apply&#8209;SPOProvisioningTemplate](ApplySPOProvisioningTemplate.md)** |Applies a provisioning template to a web
**[Get&#8209;SPOProvisioningTemplate](GetSPOProvisioningTemplate.md)** |Generates a provisioning template from a web
**[Set&#8209;SPOTheme](SetSPOTheme.md)** |Sets the theme of the current web.
##Content Types
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOContentType](RemoveSPOContentType.md)** |Removes a content type
**[Get&#8209;SPOContentType](GetSPOContentType.md)** |Retrieves a content type
**[Add&#8209;SPOContentType](AddSPOContentType.md)** |Adds a new content type
**[Remove&#8209;SPOContentTypeFromList](RemoveSPOContentTypeFromList.md)** |Removes a content type from a list
**[Add&#8209;SPOContentTypeToList](AddSPOContentTypeToList.md)** |Adds a new content type to a list
**[Set&#8209;SPODefaultContentTypeToList](SetSPODefaultContentTypeToList.md)** |Sets the default content type for a list
**[Add&#8209;SPOFieldToContentType](AddSPOFieldToContentType.md)** |Adds an existing site column to a content type
##Document Sets
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOContentTypeFromDocumentSet](RemoveSPOContentTypeFromDocumentSet.md)** |Removes a content type from a document set
**[Add&#8209;SPOContentTypeToDocumentSet](AddSPOContentTypeToDocumentSet.md)** |Adds a content type to a document set
**[Set&#8209;SPODocumentSetField](SetSPODocumentSetField.md)** |Sets a site column from the avaiable content types to a document set
**[Get&#8209;SPODocumentSetTemplate](GetSPODocumentSetTemplate.md)** |Retrieves a document set template
##Event Receivers
Cmdlet|Description
:-----|:----------
**[Add&#8209;SPOEventReceiver](AddSPOEventReceiver.md)** |Adds a new event receiver
**[Get&#8209;SPOEventReceiver](GetSPOEventReceiver.md)** |Returns all or a specific event receiver
**[Remove&#8209;SPOEventReceiver](RemoveSPOEventReceiver.md)** |Removes/unregisters a specific event receiver
##Features
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOFeature](GetSPOFeature.md)** |Returns all activated or a specific activated feature
**[Disable&#8209;SPOFeature](DisableSPOFeature.md)** |Disables a feature
**[Enable&#8209;SPOFeature](EnableSPOFeature.md)** |Enables a feature
##Fields
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOField](RemoveSPOField.md)** |Removes a field from a list or a site
**[Get&#8209;SPOField](GetSPOField.md)** |Returns a field from a list or site
**[Add&#8209;SPOField](AddSPOField.md)** |Adds a field to a list or as a site column
**[Add&#8209;SPOFieldFromXml](AddSPOFieldFromXml.md)** |Adds a field to a list or as a site column based upon a CAML/XML field definition
**[Add&#8209;SPOTaxonomyField](AddSPOTaxonomyField.md)** |Adds a taxonomy field to a list or as a site column.
##Information Management
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOSitePolicy](GetSPOSitePolicy.md)** |Retrieves all or a specific site policy
**[Set&#8209;SPOSitePolicy](SetSPOSitePolicy.md)** |Sets a site policy
##Lists
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPODefaultColumnValues](SetSPODefaultColumnValues.md)** |Sets default column values for a document library
**[Remove&#8209;SPOList](RemoveSPOList.md)** |Deletes a list
**[Get&#8209;SPOList](GetSPOList.md)** |Returns a List object
**[New&#8209;SPOList](NewSPOList.md)** |Creates a new list
**[Set&#8209;SPOList](SetSPOList.md)** |Updates list settings
**[Add&#8209;SPOListItem](AddSPOListItem.md)** |Adds an item to a list
**[Get&#8209;SPOListItem](GetSPOListItem.md)** |Retrieves list items
**[Set&#8209;SPOListPermission](SetSPOListPermission.md)** |Sets list permissions
**[Add&#8209;SPOView](AddSPOView.md)** |Adds a view to a list
**[Get&#8209;SPOView](GetSPOView.md)** |Returns one or all views from a list
**[Remove&#8209;SPOView](RemoveSPOView.md)** |Deletes a view from a list
##Publishing
Cmdlet|Description
:-----|:----------
**[Add&#8209;SPOHtmlPublishingPageLayout](AddSPOHtmlPublishingPageLayout.md)** |Adds a HTML based publishing page layout
**[Add&#8209;SPOMasterPage](AddSPOMasterPage.md)** |Adds a Masterpage
**[Add&#8209;SPOPublishingPage](AddSPOPublishingPage.md)** |Adds a publishing page
**[Add&#8209;SPOPublishingPageLayout](AddSPOPublishingPageLayout.md)** |Adds a publishing page layout
**[Remove&#8209;SPOWikiPage](RemoveSPOWikiPage.md)** |Removes a wiki page
**[Add&#8209;SPOWikiPage](AddSPOWikiPage.md)** |Adds a wiki page
**[Get&#8209;SPOWikiPageContent](GetSPOWikiPageContent.md)** |Gets the contents/source of a wiki page
**[Set&#8209;SPOWikiPageContent](SetSPOWikiPageContent.md)** |Sets the contents of a wikipage
##Search
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOSearchConfiguration](SetSPOSearchConfiguration.md)** |Returns the search configuration
**[Get&#8209;SPOSearchConfiguration](GetSPOSearchConfiguration.md)** |Returns the search configuration
##Sites
Cmdlet|Description
:-----|:----------
**[Set&#8209;SPOAppSideLoading](SetSPOAppSideLoading.md)** |Enables the App Side Loading Feature on a site
**[Get&#8209;SPOSite](GetSPOSite.md)** |Returns the current site collection from the context.
**[Uninstall&#8209;SPOSolution](UninstallSPOSolution.md)** |Uninstalls a sandboxed solution from a site collection
**[Install&#8209;SPOSolution](InstallSPOSolution.md)** |Installs a sandboxed solution to a site collection
##Taxonomy
Cmdlet|Description
:-----|:----------
**[Import&#8209;SPOTaxonomy](ImportSPOTaxonomy.md)** |Imports a taxonomy from either a string array or a file
**[Export&#8209;SPOTaxonomy](ExportSPOTaxonomy.md)** |Exports a taxonomy to either the output or to a file.
**[Set&#8209;SPOTaxonomyFieldValue](SetSPOTaxonomyFieldValue.md)** |Sets a taxonomy term value in a listitem field
**[Get&#8209;SPOTaxonomyItem](GetSPOTaxonomyItem.md)** |Returns a taxonomy item
**[Get&#8209;SPOTaxonomySession](GetSPOTaxonomySession.md)** |Returns a taxonomy session
**[New&#8209;SPOTermGroup](NewSPOTermGroup.md)** |Creates a taxonomy term group
**[Get&#8209;SPOTermGroup](GetSPOTermGroup.md)** |Returns a taxonomy term group
**[Import&#8209;SPOTermGroupFromXml](ImportSPOTermGroupFromXml.md)** |Imports a taxonomy TermGroup from either the input or from an XML file.
**[Export&#8209;SPOTermGroupToXml](ExportSPOTermGroupToXml.md)** |Exports a taxonomy TermGroup to either the output or to an XML file.
**[Import&#8209;SPOTermSet](ImportSPOTermSet.md)** |Imports a taxonomy term set from a file in the standard format.
##Tenant Administration
Cmdlet|Description
:-----|:----------
**[New&#8209;SPOTenantSite](NewSPOTenantSite.md)** |Creates a new site collection for the current tenant
**[Get&#8209;SPOTimeZoneId](GetSPOTimeZoneId.md)** |Returns a time zone ID
##User and group management
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOGroup](RemoveSPOGroup.md)** |Removes a group.
**[Set&#8209;SPOGroup](SetSPOGroup.md)** |Updates a group
**[New&#8209;SPOGroup](NewSPOGroup.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Get&#8209;SPOGroup](GetSPOGroup.md)** |Returns a specific group or all groups.
**[New&#8209;SPOUser](NewSPOUser.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Remove&#8209;SPOUserFromGroup](RemoveSPOUserFromGroup.md)** |Removes a user from a group
**[Add&#8209;SPOUserToGroup](AddSPOUserToGroup.md)** |Adds a user to a group
##User Profiles
Cmdlet|Description
:-----|:----------
**[Get&#8209;SPOUserProfileProperty](GetSPOUserProfileProperty.md)** |Office365 only: Uses the tenant API to retrieve site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
##Utilities
Cmdlet|Description
:-----|:----------
**[Send&#8209;SPOMail](SendSPOMail.md)** |Sends an email using the Office 365 SMTP Service
##Web Parts
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOWebPart](RemoveSPOWebPart.md)** |Removes a webpart from a page
**[Get&#8209;SPOWebPart](GetSPOWebPart.md)** |Returns a webpart definition object
**[Get&#8209;SPOWebPartProperty](GetSPOWebPartProperty.md)** |Returns a web part property
**[Set&#8209;SPOWebPartProperty](SetSPOWebPartProperty.md)** |Sets a web part property
**[Add&#8209;SPOWebPartToWebPartPage](AddSPOWebPartToWebPartPage.md)** |Adds a webpart to a web part page in a specified zone
**[Add&#8209;SPOWebPartToWikiPage](AddSPOWebPartToWikiPage.md)** |Adds a webpart to a wiki page in a specified table row and column
##Webs
Cmdlet|Description
:-----|:----------
**[Remove&#8209;SPOFile](RemoveSPOFile.md)** |Removes a file.
**[Get&#8209;SPOFile](GetSPOFile.md)** |Downloads a file.
**[Find&#8209;SPOFile](FindSPOFile.md)** |Finds a file in the virtual file system of the web.
**[Add&#8209;SPOFile](AddSPOFile.md)** |Uploads a file to Web
**[Set&#8209;SPOFileCheckedIn](SetSPOFileCheckedIn.md)** |Checks in a file
**[Set&#8209;SPOFileCheckedOut](SetSPOFileCheckedOut.md)** |Checks out a file
**[Add&#8209;SPOFolder](AddSPOFolder.md)** |Creates a folder within a parent folder
**[Get&#8209;SPOHomePage](GetSPOHomePage.md)** |Returns the URL to the home page
**[Set&#8209;SPOIndexedProperties](SetSPOIndexedProperties.md)** |Marks values of the propertybag to be indexed by search. Notice that this will overwrite the existing flags, e.g. only the properties you define with the cmdlet will be indexed.
**[Remove&#8209;SPOIndexedProperty](RemoveSPOIndexedProperty.md)** |Removes a key from propertybag to be indexed by search. The key and it's value retain in the propertybag, however it will not be indexed anymore.
**[Add&#8209;SPOIndexedProperty](AddSPOIndexedProperty.md)** |Marks the value of the propertybag key to be indexed by search.
**[Get&#8209;SPOIndexedPropertyKeys](GetSPOIndexedPropertyKeys.md)** |Returns the keys of the property bag values that have been marked for indexing by search
**[Get&#8209;SPOMasterPage](GetSPOMasterPage.md)** |Returns the URLS of the default Master Page and the custom Master Page.
**[Get&#8209;SPOPropertyBag](GetSPOPropertyBag.md)** |Returns the property bag values.
**[Remove&#8209;SPOPropertyBagValue](RemoveSPOPropertyBagValue.md)** |Removes a value from the property bag
**[Set&#8209;SPOPropertyBagValue](SetSPOPropertyBagValue.md)** |Sets a property bag value
**[Request&#8209;SPOReIndexWeb](RequestSPOReIndexWeb.md)** |Marks the web for full indexing during the next incremental crawl
**[Get&#8209;SPOSubWebs](GetSPOSubWebs.md)** |Returns the subwebs
**[Remove&#8209;SPOWeb](RemoveSPOWeb.md)** |Removes a subweb in the current web
**[Get&#8209;SPOWeb](GetSPOWeb.md)** |Returns the current web object
**[New&#8209;SPOWeb](NewSPOWeb.md)** |Creates a new subweb to the current web
**[Set&#8209;SPOWeb](SetSPOWeb.md)** |Sets properties on a web
##Workflows
Cmdlet|Description
:-----|:----------
**[Add&#8209;SPOWorkflowDefinition](AddSPOWorkflowDefinition.md)** |Adds a workflow definition
**[Remove&#8209;SPOWorkflowDefinition](RemoveSPOWorkflowDefinition.md)** |Removes a workflow definition
**[Get&#8209;SPOWorkflowDefinition](GetSPOWorkflowDefinition.md)** |Returns a workflow definition
**[Resume&#8209;SPOWorkflowInstance](ResumeSPOWorkflowInstance.md)** |Resumes a previously stopped workflow instance
**[Stop&#8209;SPOWorkflowInstance](StopSPOWorkflowInstance.md)** |Stops a workflow instance
**[Remove&#8209;SPOWorkflowSubscription](RemoveSPOWorkflowSubscription.md)** |Removes a workflow subscription
**[Add&#8209;SPOWorkflowSubscription](AddSPOWorkflowSubscription.md)** |Adds a workflow subscription to a list
**[Get&#8209;SPOWorkflowSubscription](GetSPOWorkflowSubscription.md)** |Returns a workflow subscriptions from a list
