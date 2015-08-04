#Set-SPOWebPartProperty
*Topic automatically generated on: 2015-08-04*

Sets a web part property
##Syntax
```powershell
Set-SPOWebPartProperty -PageUrl [<String>] -Identity [<GuidPipeBind>] -Key [<String>] -Value [<PSObject>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Key|String|True|
PageUrl|String|True|
Value|PSObject|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
