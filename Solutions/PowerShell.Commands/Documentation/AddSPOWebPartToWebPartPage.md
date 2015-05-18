#Add-SPOWebPartToWebPartPage
*Topic automatically generated on: 2015-04-29*

Adds a webpart to a web part page in a specified zone
##Syntax
```powershell
Add-SPOWebPartToWebPartPage -Xml <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]```
&nbsp;

```powershell
Add-SPOWebPartToWebPartPage -Path <String> -PageUrl <String> -ZoneId <String> -ZoneIndex <Int32> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
PageUrl|String|True|
Path|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
Xml|String|True|
ZoneId|String|True|
ZoneIndex|Int32|True|
<!-- Ref: 7634C7B82EE67146783C33033A46368E -->