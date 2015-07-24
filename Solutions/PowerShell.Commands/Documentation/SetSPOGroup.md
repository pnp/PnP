#Set-SPOGroup
*Topic automatically generated on: 2015-07-18*

Updates a group
##Syntax
```powershell
Set-SPOGroup -Identity <GroupPipeBind> [-SetAssociatedGroup <AssociatedGroupType>] [-AddRole <String>] [-RemoveRole <String>] [-Title <String>] [-Owner <String>] [-Description <String>] [-AllowRequestToJoinLeave <Boolean>] [-AutoAcceptRequestToJoinLeave <Boolean>] [-AllowMembersEditMembership <Boolean>] [-OnlyAllowMembersViewMembership <Boolean>] [-RequestToJoinEmail <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AddRole|String|False||
|AllowMembersEditMembership|Boolean|False||
|AllowRequestToJoinLeave|Boolean|False||
|AutoAcceptRequestToJoinLeave|Boolean|False||
|Description|String|False||
|Identity|GroupPipeBind|True||
|OnlyAllowMembersViewMembership|Boolean|False||
|Owner|String|False||
|RemoveRole|String|False||
|RequestToJoinEmail|String|False||
|SetAssociatedGroup|AssociatedGroupType|False||
|Title|String|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: 8E92D99D1F177C3BA13AE1EE347C8D9B -->