#Set-SPOGroup
*Topic automatically generated on: 2015-04-02*

Updates a group
##Syntax
```powershell
Set-SPOGroup -Identity [<GroupPipeBind>] [-SetAssociatedGroup [<AssociatedGroupType>]] [-AddRole [<String>]] [-RemoveRole [<String>]] [-Title [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddRole|String|False|
Identity|GroupPipeBind|True|
RemoveRole|String|False|
SetAssociatedGroup|AssociatedGroupType|False|
Title|String|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
