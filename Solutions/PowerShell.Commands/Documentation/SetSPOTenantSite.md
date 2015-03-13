#Set-SPOTenantSite
*Topic automatically generated on: 2015-03-12*

Office365 only: Uses the tenant API to set site information.

You must connect to the admin website (https://:<tenant>-admin.sharepoint.com) with Connect-SPOnline in order to use this command. 

##Syntax
```powershell
Set-SPOTenantSite [-Title [<String>]] [-Sharing [<Nullable`1>]] [-StorageMaximumLevel [<Nullable`1>]] [-StorageWarningLevel [<Nullable`1>]] [-UserCodeMaximumLevel [<Nullable`1>]] [-UserCodeWarningLevel [<Nullable`1>]] [-AllowSelfServiceUpgrade [<Nullable`1>]] [-Url [<String>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AllowSelfServiceUpgrade|Nullable`1|False|
Sharing|Nullable`1|False|
StorageMaximumLevel|Nullable`1|False|
StorageWarningLevel|Nullable`1|False|
Title|String|False|
Url|String|False|The URL of the site
UserCodeMaximumLevel|Nullable`1|False|
UserCodeWarningLevel|Nullable`1|False|
