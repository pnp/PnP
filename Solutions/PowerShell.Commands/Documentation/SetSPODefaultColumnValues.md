#Set-SPODefaultColumnValues
*Topic automatically generated on: 2015-04-29*

Sets default column values for a document library
##Syntax
```powershell
Set-SPODefaultColumnValues -Field <FieldPipeBind> -Value <String[]> [-Folder <String>] [-Web <WebPipeBind>] [-List <ListPipeBind>]```
&nbsp;

##Detailed Description
Sets default column values for a document library, per folder, or for the root folder if the folder parameter has not been specified. Supports both text and taxonomy fields.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Field|FieldPipeBind|True|The internal name, id or a reference to a field
Folder|String|False|A library relative folder path, if not specified it will set the default column values on the root folder of the library ('/')
List|ListPipeBind|False|The ID, Name or Url of the list.
Value|String[]|True|A list of values. In case of a text field the values will be concatenated, separated by a semi-column. In case of a taxonomy field multiple values will added
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Set-SPODefaultColumnValues -List Documents -Field TaxKeyword -Value "Company|Locations|Stockholm"
Sets a default value for the enterprise keywords field on a library to a term called "Stockholm", located in the "Locations" term set, which is part of the "Company" term group

###Example 2
    PS:> Set-SPODefaultColumnValues -List Documents -Field MyTextField -Value "DefaultValue"
Sets a default value for the MyTextField text field on a library to a value of "DefaultValue"
<!-- Ref: 2D58E3707B52EDECE969F1DA7D8BA826 -->