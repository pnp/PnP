#Remove-SPOTenantSite
*Topic automatically generated on: 2015-04-29*

Office365 only: Removes a site collection from the current tenant
##Syntax
```powershell
Remove-SPOTenantSite [-SkipRecycleBin [<SwitchParameter>]] [-FromRecycleBin [<SwitchParameter>]] [-Force [<SwitchParameter>]] -Url <String>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Do not ask for confirmation.
FromRecycleBin|SwitchParameter|False|If specified, will search for the site in the Recycle Bin and remove it from there.
SkipRecycleBin|SwitchParameter|False|Do not add to the trashcan if selected.
Url|String|True|
<!-- Ref: AE317BF129ED7DC4FDEA5DF2BF0AB6EB -->