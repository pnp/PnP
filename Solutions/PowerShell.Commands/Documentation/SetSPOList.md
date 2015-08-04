#Set-SPOList
*Topic automatically generated on: 2015-08-04*

Updates list settings
##Syntax
```powershell
Set-SPOList -Identity [<ListPipeBind>] [-EnableContentTypes [<Boolean>]] [-BreakRoleInheritance [<SwitchParameter>]] [-CopyRoleAssignments [<SwitchParameter>]] [-ClearSubscopes [<SwitchParameter>]] [-Title [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
BreakRoleInheritance|SwitchParameter|False|
ClearSubscopes|SwitchParameter|False|
CopyRoleAssignments|SwitchParameter|False|
EnableContentTypes|Boolean|False|Set to $true to enable content types, set to $false to disable content types
Identity|ListPipeBind|True|
Title|String|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    Set-SPOList -Identity "Demo List" -EnableContentTypes $true
Switches the Enable Content Type switch on the list
