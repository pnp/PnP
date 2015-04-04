#Add-SPOJavaScriptLink
*Topic automatically generated on: 2015-04-02*

Adds a link to a JavaScript file to a web or sitecollection
##Syntax
```powershell
Add-SPOJavaScriptLink -Key [<String>] -Url [<String[]>] [-Sequence [<Int32>]] [-Scope [<CustomActionScope>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Key|String|True|
Scope|CustomActionScope|False|
Sequence|Int32|False|
Url|String[]|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
