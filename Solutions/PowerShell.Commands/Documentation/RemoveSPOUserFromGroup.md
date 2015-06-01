#Remove-SPOUserFromGroup
*Topic automatically generated on: 2015-05-22*

Removes a user from a group
##Syntax
```powershell
Remove-SPOUserFromGroup -LoginName <String> -GroupName <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
GroupName|String|True|A valid group name
LoginName|String|True|A valid login name of a user
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Remove-SPOUserFromGroup -LoginName user@company.com -GroupName 'Marketing Site Members'


<!-- Ref: 53C66729BBAFC5480C05EEF21AB41A68 -->