#Add-SPOFieldToDocumentSet
*Topic automatically generated on: 2015-07-20*

Adds a site column to a document set
##Syntax
```powershell
Add-SPOFieldToDocumentSet -ContentType <ContentTypePipeBind> -Field <FieldPipeBind> [-Scope <DocumentSetFieldScope>] [-PushDown [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|True||
|Field|FieldPipeBind|True||
|PushDown|SwitchParameter|False||
|Scope|DocumentSetFieldScope|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOFile -Path c:\temp\company.master -Folder "_catalogs/masterpage
This will upload the file company.master to the masterpage catalog

###Example 2
    PS:> Add-SPOFile -Path .\displaytemplate.html -Folder "_catalogs/masterpage/display templates/test
This will upload the file displaytemplate.html to the test folder in the display templates folder. If the test folder not exists it will create it.
<!-- Ref: FDFBF4A2A294653E33A04CB402005C3C -->