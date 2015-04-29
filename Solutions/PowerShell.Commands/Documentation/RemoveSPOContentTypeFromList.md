#Remove-SPOContentTypeFromList
*Topic automatically generated on: 2015-04-28*

Removes a content type from a list
##Syntax
```powershell
Remove-SPOContentTypeFromList -List [<ListPipeBind>] -ContentType [<ContentTypePipeBind>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
ContentType|ContentTypePipeBind|True|
List|ListPipeBind|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
