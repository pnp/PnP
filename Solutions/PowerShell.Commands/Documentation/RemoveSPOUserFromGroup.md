#Remove-SPOUserFromGroup
*Topic automatically generated on: 2015-06-11*

Removes a user from a group
##Syntax
```powershell
Remove-SPOUserFromGroup -LoginName <String> -GroupName <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|GroupName|String|True|A valid group name|
|LoginName|String|True|A valid login name of a user|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOUserFromGroup -LoginName user@company.com -GroupName 'Marketing Site Members'

<!-- Ref: D5FE441138C38D12D6217BE656D5C4DF -->