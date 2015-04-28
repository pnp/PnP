#Set-SPOWebPartProperty
*Topic automatically generated on: 2015-04-28*

Sets a web part property
##Syntax
```powershell
Set-SPOWebPartProperty -PageUrl [<String>] -Identity [<GuidPipeBind>] -Key [<String>] -Value [<Object>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Key|String|True|
PageUrl|String|True|
Value|Object|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
