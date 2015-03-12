#Set&#8209;SPODefaultContentTypeToList
*Topic automatically generated on: 2015-03-12*

Sets the default content type for a list
##Syntax
```powershell
Set&#8209;SPODefaultContentTypeToList -List [<ListPipeBind>] -ContentType [<ContentTypePipeBind>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
ContentType|ContentTypePipeBind|True|
List|ListPipeBind|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Set-SPODefaultContentTypeToList -List "Project Documents" -ContentType "Project"
This will set the Project content type (which has already been added to a list) as the default content type
