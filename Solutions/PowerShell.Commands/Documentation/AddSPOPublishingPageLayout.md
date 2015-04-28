#Add-SPOPublishingPageLayout
*Topic automatically generated on: 2015-04-28*

Adds a publishing page layout
##Syntax
```powershell
Add-SPOPublishingPageLayout -SourceFilePath [<String>] -Title [<String>] -Description [<String>] -AssociatedContentTypeID [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedContentTypeID|String|True|
Description|String|True|
SourceFilePath|String|True|Path to the file which will be uploaded
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
