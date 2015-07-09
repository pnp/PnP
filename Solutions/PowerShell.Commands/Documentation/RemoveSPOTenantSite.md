#Remove-SPOTenantSite
*Topic automatically generated on: 2015-06-03*

Office365 only: Removes a site collection from the current tenant
##Syntax
```powershell
Remove-SPOTenantSite [-SkipRecycleBin [<SwitchParameter>]] [-FromRecycleBin [<SwitchParameter>]] [-Force [<SwitchParameter>]] -Url <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False|Do not ask for confirmation.|
|FromRecycleBin|SwitchParameter|False|If specified, will search for the site in the Recycle Bin and remove it from there.|
|SkipRecycleBin|SwitchParameter|False|Do not add to the trashcan if selected.|
|Url|String|True||
<!-- Ref: 06EDA70C35BC2601663F83A4ADA9C540 -->