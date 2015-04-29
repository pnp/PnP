#Set-SPOWebPartProperty
*Topic automatically generated on: 2015-04-29*

Sets a web part property
##Syntax
```powershell
Set-SPOWebPartProperty -PageUrl <String> -Identity <GuidPipeBind> -Key <String> -Value <Object> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Key|String|True|
PageUrl|String|True|
Value|Object|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: CB2CFF9E9325654BF73A897D7A9A8B6A -->