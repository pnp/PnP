#Add-SPOFieldToContentType
*Topic automatically generated on: 2015-06-03*

Adds an existing site column to a content type
##Syntax
```powershell
Add-SPOFieldToContentType -Field <FieldPipeBind> -ContentType <ContentTypePipeBind> [-Required [<SwitchParameter>]] [-Hidden [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|True||
|Field|FieldPipeBind|True||
|Hidden|SwitchParameter|False||
|Required|SwitchParameter|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOFieldToContentType -Field "Project_Name" -ContentType "Project Document"
This will add an existing site column with an internal name of "Project_Name" to a content type called "Project Document"
<!-- Ref: 62ABDDC0E8FAB7BC751E4DE02A8C2C05 -->