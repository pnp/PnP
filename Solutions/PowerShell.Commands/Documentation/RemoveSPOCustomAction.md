#Remove-SPOCustomAction
*Topic automatically generated on: 2015-04-28*

Removes a custom action
##Syntax
```powershell
Remove-SPOCustomAction [-Scope [<CustomActionScope>]] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]] -Identity [<GuidPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|GuidPipeBind|True|
Scope|CustomActionScope|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
