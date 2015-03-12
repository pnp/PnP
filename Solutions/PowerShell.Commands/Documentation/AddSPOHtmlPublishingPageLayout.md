#Add&#8209;SPOHtmlPublishingPageLayout
*Topic automatically generated on: 2015-03-12*

Adds a HTML based publishing page layout
##Syntax
```powershell
Add&#8209;SPOHtmlPublishingPageLayout -SourceFilePath [<String>] -Title [<String>] -Description [<String>] -AssociatedContentTypeID [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedContentTypeID|String|True|
Description|String|True|
SourceFilePath|String|True|Full path to the file which will be uploaded
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
