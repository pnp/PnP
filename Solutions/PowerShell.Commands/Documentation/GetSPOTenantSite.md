#Get-SPOTenantSite
*Topic automatically generated on: 2015-06-03*

Office365 only: Uses the tenant API to retrieve site information.

##Syntax
```powershell
Get-SPOTenantSite [-Detailed [<SwitchParameter>]] [-IncludeOneDriveSites [<SwitchParameter>]] [-Force [<SwitchParameter>]] [-Url <String>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Detailed|SwitchParameter|False||
|Force|SwitchParameter|False||
|IncludeOneDriveSites|SwitchParameter|False||
|Url|String|False|The URL of the site|
##Examples

###Example 1
    
PS:> Get-SPOTenantSite
Returns all site collections

###Example 2
    
PS:> Get-SPOTenantSite -Url http://tenant.sharepoint.com/sites/projects
Returns information about the project site.
<!-- Ref: F10FD49F4107100F7BA44DE5A40A7CA2 -->