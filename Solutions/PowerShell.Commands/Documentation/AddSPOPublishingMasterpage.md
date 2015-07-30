#Add-SPOPublishingMasterpage
*Topic automatically generated on: 2015-07-24*

Adds a Masterpage
##Syntax
```powershell
Add-SPOPublishingMasterpage -SourceFilePath <String> -Title <String> -Description <String> [-DestinationFolderHierarchy <String>] [-UiVersion <String>] [-DefaultCssFile <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|DefaultCssFile|String|False|Defautl CSS file for MasterPage|
|Description|String|True|Description for the page layout|
|DestinationFolderHierarchy|String|False|Folder hierarchy where the MasterPage layouts will be deployed|
|SourceFilePath|String|True|Path to the file which will be uploaded|
|Title|String|True|Title for the page layout|
|UiVersion|String|False|UiVersion Masterpage. Default = 15|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOPublishingMasterpage -SourceFilePath "page.master" -Title "MasterPage" -Description "MasterPage for Web" -DestinationFolderHierarchy "SubFolder" -Template "STS#0"
Add's a MasterPage to the web
<!-- Ref: EF507B608F986E7127B70ED7134EDE97 -->