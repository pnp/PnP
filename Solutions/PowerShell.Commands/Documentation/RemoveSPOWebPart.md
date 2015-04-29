#Remove-SPOWebPart
*Topic automatically generated on: 2015-04-29*

Removes a webpart from a page
##Syntax
```powershell
Remove-SPOWebPart -Identity <GuidPipeBind> -PageUrl <String> [-Web <WebPipeBind>]```
&nbsp;

```powershell
Remove-SPOWebPart -Name <String> -PageUrl <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Name|String|True|
PageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: C46AFE24E28CA395E2477D23E584D4AE -->