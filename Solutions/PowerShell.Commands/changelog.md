# OfficeDevPnP.PowerShell Changelog #

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
