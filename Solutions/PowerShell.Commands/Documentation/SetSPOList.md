#Set-SPOList
*Topic last generated: 2015-02-08*


##Syntax
    Set-SPOList -Identity [<ListPipeBind>] [-BreakRoleInheritance [<SwitchParameter>]] [-CopyRoleAssignments [<SwitchParameter>]] [-ClearSubscopes [<SwitchParameter>]] [-Title [<String>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
BreakRoleInheritance|SwitchParameter|False|
ClearSubscopes|SwitchParameter|False|
CopyRoleAssignments|SwitchParameter|False|
Identity|ListPipeBind|True|
Title|String|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
