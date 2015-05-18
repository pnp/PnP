#Remove-SPOContentType
*Topic automatically generated on: 2015-04-29*

Removes a content type
##Syntax
```powershell
Remove-SPOContentType [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -Identity <ContentTypePipeBind>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|ContentTypePipeBind|True|The name or ID of the content type to remove
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Remove-SPOContentType -Identity "Project Document"

<!-- Ref: B84AC3C9E9B9CC10254CA5458F2D1D3B -->