# OfficeDevPnP.PowerShell Changelog #

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
* Simplified connection code, added functionality to connect with App Id and App Secret. 
* Added connection samples in samples folder. 
* Added Get-SPORealm command.

**2014-08-22**
* Namespace change from OfficeDevPnP.SPOnline to OfficeDevPnP.PowerShell
