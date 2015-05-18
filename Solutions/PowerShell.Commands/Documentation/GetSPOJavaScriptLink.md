#Get-SPOJavaScriptLink
*Topic automatically generated on: 2015-04-29*

Returns all or a specific custom action(s) with location type ScriptLink
##Syntax
```powershell
Get-SPOJavaScriptLink [-Scope <CustomActionScope>] [-Web <WebPipeBind>] [-Name <String>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|False|Name of the Javascript link. Omit this parameter to retrieve all script links
Scope|CustomActionScope|False|Scope of the action, either Web (default) or Site
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 482727BA46E69567A1126DF2F93B799B -->