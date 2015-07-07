#Add-SPOWebPartToWebPartPage
*Topic automatically generated on: 2015-06-11*

Adds a webpart to a web part page in a specified zone
##Syntax
```powershell
Add-SPOWebPartToWebPartPage -Path <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]
```


```powershell
Add-SPOWebPartToWebPartPage -Xml <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|PageUrl|String|True||
|Path|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
|Xml|String|True||
|ZoneId|String|True||
|ZoneIndex|Int32|True||
<!-- Ref: AF25CB5A96597A73DE3C6224A3A5E82E -->