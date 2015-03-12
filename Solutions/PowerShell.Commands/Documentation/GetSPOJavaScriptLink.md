#Get&#8209;SPOJavaScriptLink
*Topic automatically generated on: 2015-03-12*

Returns all or a specific custom action(s) with location type ScriptLink
##Syntax
```powershell
Get&#8209;SPOJavaScriptLink [-Scope [<CustomActionScope>]] [-Web [<WebPipeBind>]] [-Name [<String>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|False|Name of the Javascript link. Omit this parameter to retrieve all script links
Scope|CustomActionScope|False|Scope of the action, either Web (default) or Site
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
