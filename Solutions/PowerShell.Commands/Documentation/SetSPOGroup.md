#Set-SPOGroup
*Topic automatically generated on: 2015-06-11*

Updates a group
##Syntax
```powershell
Set-SPOGroup -Identity <GroupPipeBind> [-SetAssociatedGroup <AssociatedGroupType>] [-AddRole <String>] [-RemoveRole <String>] [-Title <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AddRole|String|False||
|Identity|GroupPipeBind|True||
|RemoveRole|String|False||
|SetAssociatedGroup|AssociatedGroupType|False||
|Title|String|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: 7BBF3C14CF4D5B93911759435B661E89 -->