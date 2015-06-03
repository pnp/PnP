# OfficeDevPnP.PowerShell Changelog #

**2015-06-01**
* Added Add-SPOWorkflowDefinition cmdlet
* Updated Add-SPOField to allow for -Field parameter to add a site column to a list.

**2015-05-28**
* Added Set-SPOSitePolicy and Get-SPOSitePolicy cmdlets

**2015-05-22**
* Updated Add-SPOHtlPublishingPageLayout and Add-SPOPublishingPageLayout to support DestinationFolderHierarchy parameter
* Updated Add-SPOFile to create the target folder is not present
* Updated Remove-SPOUserFromGroup to accept either a login name or an email address of a user.

**2015-05-15**
* Updated Set-SPOList to switching if ContentTypes are enabled on the list

**2015-04-24**
* Updated Get-SPOProvisioningTemplate and Apply-SPOProvisioningTemplate to show a progress bar
* Updated GEt-SPOProvisioningTemplate with optional switches to export either Site Collection Term Group (if available) or all Term Groups in the default site collection termstore.
* Added Export-SPOTermGroup cmdlet that supports the provisioning engine XML format
* Added Import-SPOTermGroup cmdlet that supports the provisioning engine XML format

**2015-04-20**
* Admin cmdlets: Get-SPOTenantSite, New-SPOTenantSite, Remove-SPOTenantSite, Set-SPOTenantSite and Get-SPOWebTemplates now automatically switch context. This means that you don't have to connect to https://<tenant>-admin.sharepoint.com first in order to execute them.
**2015-04-08**
* Added Apply-SPOProvisioningTemplate cmdlet
* Added Get-SPOPRovisioningTemplate cmdlet
* Extended Enable-SPOFeature cmdlet to handle Sandboxed features

**2015-03-11**
* Added Get-SPOJavaScript link cmdlet
* Refactored JavaScript related cmdlets to use -Name parameter instead of -Key (-Key still works for backwards compatibility reasons)
* Refactored JavaScript related cmdlets to use -Scope [Web|Site] parameter instead of -FromSite, -SiteScoped and -AddToSite parameters. The old parameters still work for backwards compatibility reasons.
* Fixed an issue in cmdlet help generation where the syntax would not be shown for cmdlets with only one parameter set.

**2015-03-10**
* Added Sequence parameter to Add-SPOJavaScriptLink and Add-SPOJavaScriptBlock cmdlets
* Added Remove-SPOFile cmdlet

**2015-02-25**
* Updated Location parameter in Add-/Remove-SPONavigationNode

**2015-01-07**
* Introduced new Cmdlet: Get-SPOWebPartProperty to return webpart properties
* Updated Set-SPOWebPartProperty cmdlet to support int values

**2015-01-02**
* Removed SetAssociatedGroup parameter from new-spogroup cmdlet and moved it to a separate cmdlet: Set-SPOGroup
* Introduced new Cmdlet: Set-SPOGroup to set the group as an associated group and optionally add or remove role assignments
* Introduced new Cmdlet: Set-SPOList to set list properties
* Introduced new Cmdlet: Set-SPOListPermission to set list permissions

**2014-12-30**
* Changed New-SPOWeb to return the actual web as an object instead of a success message.
* Added -SetAssociatedGroup parameter to New-SPOGroup to set a group as a default associated visitors, members or owners group
* Updated New-SPOGroup to allow setting groups as owners

**2014-12-01**
* Added Get-SPOListItem cmdlet to retrieve list items by id, unique id, or CAML. Optionally you can define which fields to load.
**2014-11-05**
* Added Add-SPOFolder cmdlet

**2014-11-04**
* Added Get-SPOIndexedPropertyBagKeys cmdlet
* Updated Set-SPOPropertyBagValue to not remove a property from the indexed properties if it was already in the indexed properties.
* Updated Get-SPOTenantSite output formatting

**2014-11-03**
* Split up Add-SPOField into Add-SPOField and Add-SPOFieldFromXml. The latter only accepts XML input while the first takes parameters to create fields

**2014-10-15**
* Added Add-SPOWorkflowSubscription, Get-SPOWorkflowDefinition, Get-SPOWorkflowSubscription, Remove-SPOWorkflowDefinition, Remove-SPOWorkflowSubscription, Resume-SPOWorkflowInstance, Stop-SPOWorkflowInstance

**2014-10-14**
* Added Get-SPOUserProfileProperty cmdlet
* Added New-SPOPersonalSite cmdlet
* Fixed Get-SPOView cmdlet

**2014-10-08**
* Added Set-SPODefaultColumnValue 
**2014-09-19**
* Removed Url Parameters on Add-SPOFile and made Folder parameter mandatory.

**2014-09-06**
* Added new Set-SPOWeb cmdlet to set Title, SiteLogo, or AlternateCssUrl

**2014-09-03**
* Renamed Add-SPOApp to Import-SPOAppPackage to align with server cmdlet
* Renamed Remove-SPOApp to Uninstall-SPOAppInstance to align with server cmdlet

**2014-08-29**
* Removed OfficeDevPnP.PowerShell.Core project, not in use anymore as all cmdlets now make use of the OfficeDevPnP.Core project.

**2014-08-27**
* Split up Add-SPOWebPart in two cmdlets, Add-SPOWebPartToWikiPage and Add-SPOWebPartToWebPartPage, to reduce confusing parameter sets
* Changed parameters of Add-SPOCustomAction cmdlet
* Changed name of Add-SPONavigationLink to Add-SPONavigationNode, in sync with method name of OfficeDevPnP.Core. Changed parameters of cmdlet.


**2014-08-26**
* Updated several commands to use OfficeDevPnP.Core instead of OfficeDevPnP.PowerShell.Core
* Marked SPOSite and SPOTaxonomy as obsolete. Use OfficeDevPnP.Core extensions instead

**2014-08-23**
* Simplified connection code, added functionality to connect with add-in Id and add-in Secret. 
* Added connection samples in samples folder. 
* Added Get-SPORealm command.

**2014-08-22**
* Namespace change from OfficeDevPnP.SPOnline to OfficeDevPnP.PowerShell
