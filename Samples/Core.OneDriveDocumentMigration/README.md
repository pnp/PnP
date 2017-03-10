# ONEDRIVE DOCUMENT MIGRATION #

### Summary ###
This sample demonstrates how to migrate documents from onpremise mysite to OneDrive for Business on Office 365.

### Applies to ###
- Office 365 Multi-Tenant
- Sharepoint 2010 Mysite


### Solution ###
Solution | Author(s)
---------|----------
Contoso.Core.OneDriveDocumentMigration | Jaakko Nikko, Vesa Juvonen (Microsoft)

### Version history ###
Version  | Date | Comments
---------| -----| --------
1.0  | June 13th 2014 | Initial release

### Disclaimer ###
**THIS CODE IS PROVIDED *AS IS* WITHOUT WARRANTY OF ANY KIND, EITHER EXPRESS OR IMPLIED, INCLUDING ANY IMPLIED WARRANTIES OF FITNESS FOR A PARTICULAR PURPOSE, MERCHANTABILITY, OR NON-INFRINGEMENT.**


----------

# GENERAL COMMENTS #
As part of the new Client Side Object Model (CSOM) assemblies released in the [SharePoint Online Client Components SDK](http://www.microsoft.com/en-us/download/details.aspx?id=42038), we now have many new capabilities and improvements. One specifically is the capability to programmatically upload files to One Drive for Business sites in Office 365.

Migrating users’ files from existing SharePoint 2010 MySite to Office One Drive for Business sites in Office 365 can be done manually which is not very feasible solution when multiple thousand users and files are concerned. This solution is build for automation where a lot of users with a lot of files are to be migrated. This solution reads folder structure from MySite (Personal Document, Shared Documents and Shared Pictures) with sub folders and creates equivalent structure and mapping in OneDrive. If user has created other lists in MySite, this solution does not take those into account. The solution can be run multiple times as there is an attribute to not overwrite existing files.

The users and their Mysite/OneDrive tokens are provided via comma-separated file (.csv, sample attached). The same csv file could be used for automated OneDrive for Business site promotion (small changes to Core.OneDriveProvision needed, however).


### CSV FILE EXAMPLE ###
CSV file containing relevat user information. First line is removed in solution.

SpoOneDriveUserName | SpoOneDriveUserEmail | OnPremUserName
--------------------|----------------------|-----------------
frodobanks | FrodoBanks@poc.onmicrosoft.com | Plifispen
bilboburrows | BilboBurrows@poc.onmicrosoft.com | noter1958

### SOLUTION PARAMETERS ###
Command-line parameters are to be provided in order to make solution work.

_Parameters:_
```
0 - Sharepoint Online Admin Url ("https://poc-admin.sharepoint.com")
1 - Sharepoint Online Onedrive url with placeholder ("https://poc-my.sharepoint.com/personal/{0}_poc_onmicrosoft_com")
2 - Sharepoint Online Admin name ("admin@poc.onmicrosoft.com")
3 - Sharepoint Online Admin password ("pass@word1")
4 - path to CSV File (C:\temp\users.csv)
5 - Onprem mysite url with placeholder ("http://mysite/personal/{0}")
6 - Onprem admin name (admin)
7 - Onprem admin password (pass@word1)
8 - Overwrite files in SPO
```

### EXAMPLE: ###
```
Contoso.Core.OneDriveDocumentMigration.exe
	"https://poc-admin.sharepoint.com"
	"https://poc-my.sharepoint.com/personal/{0}_poc_onmicrosoft_com"
	"admin@poc.onmicrosoft.com"
	"password"
	"C:\Scripts\skriptit\DocumentMigration_Example.csv"
	"http://mysite/personal/{0}"
	"administrator"
	"password"
	false
```

<img src="https://telemetry.sharepointpnp.com/pnp/samples/Core.OneDriveDocumentMigration" />