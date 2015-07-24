#Remove-SPOFile
*Topic automatically generated on: 2015-06-11*

Removes a file.
##Syntax
```powershell
Remove-SPOFile [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -ServerRelativeUrl <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|ServerRelativeUrl|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:>Remove-SPOFile -ServerRelativeUrl /sites/project/_catalogs/themes/15/company.spcolor

<!-- Ref: A14A2234B2C046B5DEB48F99F0D4D831 -->