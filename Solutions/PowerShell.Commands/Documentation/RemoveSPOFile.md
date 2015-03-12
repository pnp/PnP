#Remove&#8209;SPOFile
*Topic automatically generated on: 2015-03-12*

Removes a file.
##Syntax
```powershell
Remove&#8209;SPOFile [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]] -ServerRelativeUrl [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
ServerRelativeUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:>Remove-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor

