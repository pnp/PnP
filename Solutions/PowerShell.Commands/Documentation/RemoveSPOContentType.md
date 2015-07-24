#Remove-SPOContentType
*Topic automatically generated on: 2015-06-11*

Removes a content type
##Syntax
```powershell
Remove-SPOContentType [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -Identity <ContentTypePipeBind>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|Identity|ContentTypePipeBind|True|The name or ID of the content type to remove|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOContentType -Identity "Project Document"

<!-- Ref: 92371ECF2857E3013335C3C2C3461C9E -->