#Remove-SPOFile
*Topic automatically generated on: 2015-04-29*

Removes a file.
##Syntax
```powershell
Remove-SPOFile [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -ServerRelativeUrl <String>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
ServerRelativeUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:>Remove-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor

<!-- Ref: E742B88F460D301EDFE4A66999E5C806 -->