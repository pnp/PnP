#Set-SPODefaultContentTypeToList
*Topic automatically generated on: 2015-06-03*

Sets the default content type for a list
##Syntax
```powershell
Set-SPODefaultContentTypeToList -List <ListPipeBind> -ContentType <ContentTypePipeBind> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|True||
|List|ListPipeBind|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Set-SPODefaultContentTypeToList -List "Project Documents" -ContentType "Project"
This will set the Project content type (which has already been added to a list) as the default content type
<!-- Ref: B40227CF5BA926122A3198A089FDDB84 -->