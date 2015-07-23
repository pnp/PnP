#Remove-SPOGroup
*Topic automatically generated on: 2015-07-22*

Removes a group.
##Syntax
```powershell
Remove-SPOGroup [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] [-Identity <GroupPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|Identity|GroupPipeBind|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOGroup -Identity "My Users"

<!-- Ref: 6676EE210DD19F365519626B1F0C6895 -->