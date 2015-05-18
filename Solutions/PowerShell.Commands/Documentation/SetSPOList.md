#Set-SPOList
*Topic automatically generated on: 2015-04-29*

Updates list settings
##Syntax
```powershell
Set-SPOList -Identity <ListPipeBind> [-BreakRoleInheritance [<SwitchParameter>]] [-CopyRoleAssignments [<SwitchParameter>]] [-ClearSubscopes [<SwitchParameter>]] [-Title <String>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
BreakRoleInheritance|SwitchParameter|False|
ClearSubscopes|SwitchParameter|False|
CopyRoleAssignments|SwitchParameter|False|
Identity|ListPipeBind|True|
Title|String|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: 261424D7AE9CC5265419D98CBDDFC9A0 -->