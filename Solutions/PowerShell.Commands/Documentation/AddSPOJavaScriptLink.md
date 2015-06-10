#Add-SPOJavaScriptLink
*Topic automatically generated on: 2015-06-03*

Adds a link to a JavaScript file to a web or sitecollection
##Syntax
```powershell
Add-SPOJavaScriptLink -Key <String> -Url <String[]> [-Sequence <Int32>] [-Scope <CustomActionScope>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Key|String|True||
|Scope|CustomActionScope|False||
|Sequence|Int32|False||
|Url|String[]|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: F034704BA5E1D1B5187AB159901C5BAC -->