#Add-SPOFieldFromXml
*Topic automatically generated on: 2015-03-12*

Adds a field to a list or as a site column based upon a CAML/XML field definition
##Syntax
```powershell
Add-SPOFieldFromXml [-List [<ListPipeBind>]] [-Web [<WebPipeBind>]] -FieldXml [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
FieldXml|String|True|CAML snippet containing the field definition. See http://msdn.microsoft.com/en-us/library/office/ms437580(v=office.15).aspx
List|ListPipeBind|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
