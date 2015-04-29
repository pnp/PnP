#Add-SPOPublishingPageLayout
*Topic automatically generated on: 2015-04-29*

Adds a publishing page layout
##Syntax
```powershell
Add-SPOPublishingPageLayout -SourceFilePath <String> -Title <String> -Description <String> -AssociatedContentTypeID <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedContentTypeID|String|True|
Description|String|True|
SourceFilePath|String|True|Path to the file which will be uploaded
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 96EAEC8914A34A033E64E0CA4AEF2C17 -->