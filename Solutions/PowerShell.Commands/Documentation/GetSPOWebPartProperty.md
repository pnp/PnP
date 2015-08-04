#Get-SPOWebPartProperty
*Topic automatically generated on: 2015-08-04*

Returns a web part property
##Syntax
```powershell
Get-SPOWebPartProperty -PageUrl [<String>] -Identity [<GuidPipeBind>] [-Key [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Key|String|False|
PageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
