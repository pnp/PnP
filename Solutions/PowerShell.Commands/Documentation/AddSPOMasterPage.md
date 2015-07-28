#Add-SPOMasterPage
*Topic automatically generated on: 2015-07-28*

Adds a Masterpage
##Syntax
```powershell
Add-SPOMasterPage -SourceFilePath <String> -Title <String> -Description <String> [-DestinationFolderHierarchy <String>] [-UiVersion <String>] [-DefaultCssFile <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|DefaultCssFile|String|False|Default CSS file for MasterPage, this Url is SiteRelative|
|Description|String|True|Description for the page layout|
|DestinationFolderHierarchy|String|False|Folder hierarchy where the MasterPage layouts will be deployed|
|SourceFilePath|String|True|Path to the file which will be uploaded|
|Title|String|True|Title for the page layout|
|UiVersion|String|False|UiVersion Masterpage. Default = 15|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOPMasterPage -SourceFilePath "page.master" -Title "MasterPage" -Description "MasterPage for Web" -DestinationFolderHierarchy "SubFolder"
Adds a MasterPage to the web
<!-- Ref: 7B5E4D1D88D6F0C83F98646EE433752D -->