#Add-SPOHtmlPublishingPageLayout
*Topic automatically generated on: 2015-04-29*

Adds a HTML based publishing page layout
##Syntax
```powershell
Add-SPOHtmlPublishingPageLayout -SourceFilePath <String> -Title <String> -Description <String> -AssociatedContentTypeID <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedContentTypeID|String|True|
Description|String|True|
SourceFilePath|String|True|Path to the file which will be uploaded
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: B7CF06C9FEDB780C7D0CED67D7CF7079 -->