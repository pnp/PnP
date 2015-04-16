#Add-SPOFieldToContentType
*Topic automatically generated on: 2015-04-02*

Adds an existing site column to a content type
##Syntax
```powershell
Add-SPOFieldToContentType -Field [<FieldPipeBind>] -ContentType [<ContentTypePipeBind>] [-Required [<SwitchParameter>]] [-Hidden [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
ContentType|ContentTypePipeBind|True|
Field|FieldPipeBind|True|
Hidden|SwitchParameter|False|
Required|SwitchParameter|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Add-SPOFieldToContentType -Field "Project_Name" -ContentType "Project Document"
This will add an existing site column with an internal name of "Project_Name" to a content type called "Project Document"
