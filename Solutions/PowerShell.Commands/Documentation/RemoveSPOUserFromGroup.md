#Remove-SPOUserFromGroup
*Topic automatically generated on: 2015-02-08*

Removes a user from a group
##Syntax
    Remove-SPOUserFromGroup -LoginName [<String>] -GroupName [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
GroupName|String|True|A valid group name
LoginName|String|True|A valid logon name of a user
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> Remove-SPOUserFromGroup -LoginName user@company.com -GroupName 'Marketing Site Members'


