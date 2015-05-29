#Add-SPOHtmlPublishingPageLayout
*Topic automatically generated on: 2015-05-25*

Adds a HTML based publishing page layout
##Syntax
```powershell
Add-SPOHtmlPublishingPageLayout -SourceFilePath <String> -Title <String> -Description <String> -AssociatedContentTypeID <String> [-DestinationFolderHierarchy <String>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedContentTypeID|String|True|Associated content type ID
Description|String|True|Description for the page layout
DestinationFolderHierarchy|String|False|Folder hierarchy where the html page layouts will be deployed
SourceFilePath|String|True|Path to the file which will be uploaded
Title|String|True|Title for the page layout
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 9BC31EA6956B72AE2412C82EDF5BE11E -->