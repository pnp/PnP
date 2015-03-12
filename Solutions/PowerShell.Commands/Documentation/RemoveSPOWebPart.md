#Remove&#8209;SPOWebPart
*Topic automatically generated on: 2015-03-12*

Removes a webpart from a page
##Syntax
```powershell
Remove&#8209;SPOWebPart -Identity [<GuidPipeBind>] -PageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Remove&#8209;SPOWebPart -Name [<String>] -PageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Name|String|True|
PageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
