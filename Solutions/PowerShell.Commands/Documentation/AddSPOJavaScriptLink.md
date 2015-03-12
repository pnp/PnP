#Add&#8209;SPOJavaScriptLink
*Topic automatically generated on: 2015-03-12*

Adds a link to a JavaScript file to a web or sitecollection
##Syntax
```powershell
Add&#8209;SPOJavaScriptLink -Key [<String>] -Url [<String[]>] [-Sequence [<Int32>]] [-Scope [<CustomActionScope>]] [-Web [<WebPipeBind>]]
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
