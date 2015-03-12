# Cmdlet Documentation #
Below you can find a list of all the available cmdlets. Many commands provide built-in help and examples. Retrieve the detailed help with 

```powershell
Get-Help Connect-SPOnline -Detailed
```


##
Cmdlet|Description
:-----|:----------
**[Get-SPOAuthenticationRealm](Documentation/GetSPOAuthenticationRealm.md)** |Gets the authentication realm for the current web
**[Remove-SPOContentType](Documentation/RemoveSPOContentType.md)** |Removes a content type
**[Get-SPOContentType](Documentation/GetSPOContentType.md)** |Retrieves a content type
**[Add-SPOContentType](Documentation/AddSPOContentType.md)** |Adds a new content type
**[Add-SPOContentTypeToList](Documentation/AddSPOContentTypeToList.md)** |Adds a new content type to a list
**[Set-SPODefaultColumnValues](Documentation/SetSPODefaultColumnValues.md)** |Sets default column values for a document library
**[Set-SPODefaultContentTypeToList](Documentation/SetSPODefaultContentTypeToList.md)** |Sets the default content type for a list
**[Add-SPOEventReceiver](Documentation/AddSPOEventReceiver.md)** |Adds a new event receiver
**[Get-SPOEventReceiver](Documentation/GetSPOEventReceiver.md)** |Returns all or a specific event receiver
**[Remove-SPOEventReceiver](Documentation/RemoveSPOEventReceiver.md)** |Removes/unregisters a specific event receiver
**[Get-SPOFeature](Documentation/GetSPOFeature.md)** |Returns all or a specific feature
**[Disable-SPOFeature](Documentation/DisableSPOFeature.md)** |Disables a feature
**[Enable-SPOFeature](Documentation/EnableSPOFeature.md)** |Enables a feature
**[Add-SPOFieldToContentType](Documentation/AddSPOFieldToContentType.md)** |Adds an existing site column to a content type
**[Remove-SPOFile](Documentation/RemoveSPOFile.md)** |Removes a file.
**[Get-SPOFile](Documentation/GetSPOFile.md)** |Downloads a file.
**[Find-SPOFile](Documentation/FindSPOFile.md)** |Finds a file in the virtual file system of the web.
**[Add-SPOFile](Documentation/AddSPOFile.md)** |Uploads a file to Web
**[Add-SPOFolder](Documentation/AddSPOFolder.md)** |Creates a folder within a parent folder
**[New-SPOGroup](Documentation/NewSPOGroup.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Get-SPOGroup](Documentation/GetSPOGroup.md)** |Returns a specific group or all groups.
**[Get-SPOList](Documentation/GetSPOList.md)** |Returns a List object
**[New-SPOList](Documentation/NewSPOList.md)** |Creates a new list
**[Get-SPOListItem](Documentation/GetSPOListItem.md)** |Retrieves list items
**[Set-SPOMinimalDownloadStrategy](Documentation/SetSPOMinimalDownloadStrategy.md)** |Activates or deactivates the minimal downloading strategy.
**[Get-SPOSite](Documentation/GetSPOSite.md)** |Returns the current site collection from the context.
**[Uninstall-SPOSolution](Documentation/UninstallSPOSolution.md)** |Uninstalls a sandboxed solution from a site collection
**[Install-SPOSolution](Documentation/InstallSPOSolution.md)** |Installs a sandboxed solution to a site collection
**[Import-SPOTaxonomy](Documentation/ImportSPOTaxonomy.md)** |Imports a taxonomy from either a string array or a file
**[Export-SPOTaxonomy](Documentation/ExportSPOTaxonomy.md)** |Exports a taxonomy to either the output or to a file.
**[Set-SPOTaxonomyFieldValue](Documentation/SetSPOTaxonomyFieldValue.md)** |Sets a taxonomy term value in a listitem field
**[New-SPOUser](Documentation/NewSPOUser.md)** |Adds a user to the build-in Site User Info List and returns a user object
**[Remove-SPOUserFromGroup](Documentation/RemoveSPOUserFromGroup.md)** |Removes a user from a group
**[Get-SPOUserProfileProperty](Documentation/GetSPOUserProfileProperty.md)** |Office365 only: Uses the tenant API to retrieve site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
**[Add-SPOUserToGroup](Documentation/AddSPOUserToGroup.md)** |Adds a user to a group
**[New-SPOWeb](Documentation/NewSPOWeb.md)** |Creates a new subweb to the current web
**[Set-SPOWeb](Documentation/SetSPOWeb.md)** |Sets properties on a web
##
Cmdlet|Description
:-----|:----------
**[Set-SPOAppSideLoading](Documentation/SetSPOAppSideLoading.md)** |
**[Remove-SPOCustomAction](Documentation/RemoveSPOCustomAction.md)** |
**[Remove-SPOField](Documentation/RemoveSPOField.md)** |
**[Get-SPOField](Documentation/GetSPOField.md)** |
**[Add-SPOField](Documentation/AddSPOField.md)** |
**[Add-SPOFieldFromXml](Documentation/AddSPOFieldFromXml.md)** |
**[Set-SPOFileCheckedIn](Documentation/SetSPOFileCheckedIn.md)** |
**[Set-SPOFileCheckedOut](Documentation/SetSPOFileCheckedOut.md)** |
**[Set-SPOGroup](Documentation/SetSPOGroup.md)** |
**[Get-SPOHomePage](Documentation/GetSPOHomePage.md)** |
**[Add-SPOHtmlPublishingPageLayout](Documentation/AddSPOHtmlPublishingPageLayout.md)** |
**[Set-SPOIndexedProperties](Documentation/SetSPOIndexedProperties.md)** |
**[Get-SPOIndexedPropertyKeys](Documentation/GetSPOIndexedPropertyKeys.md)** |
**[Remove-SPOList](Documentation/RemoveSPOList.md)** |
**[Set-SPOList](Documentation/SetSPOList.md)** |
**[Set-SPOListPermission](Documentation/SetSPOListPermission.md)** |
**[Send-SPOMail](Documentation/SendSPOMail.md)** |
**[Get-SPOMasterPage](Documentation/GetSPOMasterPage.md)** |
**[New-SPOPersonalSite](Documentation/NewSPOPersonalSite.md)** |
**[Get-SPOPropertyBag](Documentation/GetSPOPropertyBag.md)** |
**[Remove-SPOPropertyBagValue](Documentation/RemoveSPOPropertyBagValue.md)** |
**[Set-SPOPropertyBagValue](Documentation/SetSPOPropertyBagValue.md)** |
**[Add-SPOPublishingPage](Documentation/AddSPOPublishingPage.md)** |
**[Add-SPOPublishingPageLayout](Documentation/AddSPOPublishingPageLayout.md)** |
**[Request-SPOReIndexWeb](Documentation/RequestSPOReIndexWeb.md)** |
**[Get-SPOSubWebs](Documentation/GetSPOSubWebs.md)** |
**[Add-SPOTaxonomyField](Documentation/AddSPOTaxonomyField.md)** |
**[Get-SPOTaxonomyItem](Documentation/GetSPOTaxonomyItem.md)** |
**[Get-SPOTaxonomySession](Documentation/GetSPOTaxonomySession.md)** |
**[Get-SPOWeb](Documentation/GetSPOWeb.md)** |
**[Remove-SPOWebPart](Documentation/RemoveSPOWebPart.md)** |
**[Get-SPOWebPart](Documentation/GetSPOWebPart.md)** |
**[Get-SPOWebPartProperty](Documentation/GetSPOWebPartProperty.md)** |
**[Set-SPOWebPartProperty](Documentation/SetSPOWebPartProperty.md)** |
**[Add-SPOWebPartToWebPartPage](Documentation/AddSPOWebPartToWebPartPage.md)** |
**[Add-SPOWebPartToWikiPage](Documentation/AddSPOWebPartToWikiPage.md)** |
**[Add-SPOView](Documentation/AddSPOView.md)** |
**[Get-SPOView](Documentation/GetSPOView.md)** |
**[Remove-SPOView](Documentation/RemoveSPOView.md)** |
**[Remove-SPOWikiPage](Documentation/RemoveSPOWikiPage.md)** |
**[Add-SPOWikiPage](Documentation/AddSPOWikiPage.md)** |
**[Get-SPOWikiPageContent](Documentation/GetSPOWikiPageContent.md)** |
**[Set-SPOWikiPageContent](Documentation/SetSPOWikiPageContent.md)** |
**[Remove-SPOWorkflowDefinition](Documentation/RemoveSPOWorkflowDefinition.md)** |
**[Get-SPOWorkflowDefinition](Documentation/GetSPOWorkflowDefinition.md)** |
**[Resume-SPOWorkflowInstance](Documentation/ResumeSPOWorkflowInstance.md)** |
**[Stop-SPOWorkflowInstance](Documentation/StopSPOWorkflowInstance.md)** |
**[Remove-SPOWorkflowSubscription](Documentation/RemoveSPOWorkflowSubscription.md)** |
**[Add-SPOWorkflowSubscription](Documentation/AddSPOWorkflowSubscription.md)** |
**[Get-SPOWorkflowSubscription](Documentation/GetSPOWorkflowSubscription.md)** |
##Apps
Cmdlet|Description
:-----|:----------
**[Uninstall-SPOAppInstance](Documentation/UninstallSPOAppInstance.md)** |Removes an app from a site
**[Get-SPOAppInstance](Documentation/GetSPOAppInstance.md)** |Returns a SharePoint App Instance
**[Import-SPOAppPackage](Documentation/ImportSPOAppPackage.md)** |Adds a SharePoint App to a site
##Base Cmdlets
Cmdlet|Description
:-----|:----------
**[Set-SPOConfiguration](Documentation/SetSPOConfiguration.md)** |To be deprecated
**[Get-SPOConfiguration](Documentation/GetSPOConfiguration.md)** |To be deprecated
**[Get-SPOContext](Documentation/GetSPOContext.md)** |Returns a Client Side Object Model context
**[Get-SPOHealthScore](Documentation/GetSPOHealthScore.md)** |Retrieves the current health score value of the server
**[Disconnect-SPOnline](Documentation/DisconnectSPOnline.md)** |Disconnects the context
**[Connect-SPOnline](Documentation/ConnectSPOnline.md)** |Connects to a SharePoint site and creates an in-memory context
**[Execute-SPOQuery](Documentation/ExecuteSPOQuery.md)** |Executes any queued actions / changes on the SharePoint Client Side Object Model Context
**[Get-SPOStoredCredential](Documentation/GetSPOStoredCredential.md)** |Returns a stored credential from the Windows Credential Manager
##Branding
Cmdlet|Description
:-----|:----------
**[Get-SPOCustomAction](Documentation/GetSPOCustomAction.md)** |Returns all or a specific custom action(s)
**[Add-SPOCustomAction](Documentation/AddSPOCustomAction.md)** |Adds a custom action to a web
**[Set-SPOHomePage](Documentation/SetSPOHomePage.md)** |Sets the home page of the current web.
**[Add-SPOJavaScriptBlock](Documentation/AddSPOJavaScriptBlock.md)** |Adds a link to a JavaScript snippet/block to a web or site collection
**[Get-SPOJavaScriptLink](Documentation/GetSPOJavaScriptLink.md)** |Returns all or a specific custom action(s) with location type ScriptLink
**[Remove-SPOJavaScriptLink](Documentation/RemoveSPOJavaScriptLink.md)** |Removes a JavaScript link or block from a web or sitecollection
**[Add-SPOJavaScriptLink](Documentation/AddSPOJavaScriptLink.md)** |Adds a link to a JavaScript file to a web or sitecollection
**[Set-SPOMasterPage](Documentation/SetSPOMasterPage.md)** |Sets the default master page of the current web.
**[Add-SPONavigationNode](Documentation/AddSPONavigationNode.md)** |Adds a menu item to either the quicklaunch or top navigation
**[Remove-SPONavigationNode](Documentation/RemoveSPONavigationNode.md)** |Removes a menu item from either the quicklaunch or top navigation
**[Set-SPOTheme](Documentation/SetSPOTheme.md)** |Sets the theme of the current web.
##Tenant Administration
Cmdlet|Description
:-----|:----------
**[Set-SPOTenantSite](Documentation/SetSPOTenantSite.md)** |Office365 only: Uses the tenant API to set site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
**[Get-SPOTenantSite](Documentation/GetSPOTenantSite.md)** |Office365 only: Uses the tenant API to retrieve site information.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
**[Remove-SPOTenantSite](Documentation/RemoveSPOTenantSite.md)** |Office365 only: Removes a site collection from the current tenant
**[New-SPOTenantSite](Documentation/NewSPOTenantSite.md)** |Office365 only: Creates a new site collection for the current tenant
**[Get-SPOTimeZoneId](Documentation/GetSPOTimeZoneId.md)** |Returns a time zone ID
**[Get-SPOWebTemplates](Documentation/GetSPOWebTemplates.md)** |Office365 only: Returns the available web templates.  You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command.  
