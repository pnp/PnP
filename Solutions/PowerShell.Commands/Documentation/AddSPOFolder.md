#Add-SPOFolder
*Topic automatically generated on: 2015-06-04*

Creates a folder within a parent folder
##Syntax
```powershell
Add-SPOFolder -Name <String> -Folder <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Folder|String|True|The parent folder in the site|
|Name|String|True|The folder name|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOFolder -Name NewFolder -Folder _catalogs/masterpage/newfolder

<!-- Ref: CBBCDA0CA4186E50F84F4F12CA6A27DA -->