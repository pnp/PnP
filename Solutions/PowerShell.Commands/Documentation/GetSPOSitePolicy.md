#Get-SPOSitePolicy
*Topic automatically generated on: 2015-08-04*

Retrieves all or a specific site policy
##Syntax
```powershell
Get-SPOSitePolicy [-AllAvailable [<SwitchParameter>]] [-Name [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AllAvailable|SwitchParameter|False|Retrieve all available site policies
Name|String|False|Retrieves a site policy with a specific name
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Get-SPOSitePolicy
Retrieves the current applied site policy.

###Example 2
    PS:> Get-SPOSitePolicy -AllAvailable
Retrieves all available site policies.

###Example 3
    PS:> Get-SPOSitePolicy -Name "Contoso HBI"
Retrieves an available site policy with the name "Contoso HBI".
