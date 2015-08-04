#Remove-SPOCustomAction
*Topic automatically generated on: 2015-08-04*

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
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
