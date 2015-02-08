#Add-SPOUserToGroup
*Topic last generated: 2015-02-08*

Adds a user to a group
##Syntax
    Add-SPOUserToGroup -LoginName [<String>] -Identity [<GroupPipeBind>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GroupPipeBind|True|The group id, group name or group object to add the user to.
LoginName|String|True|The login name of the user
Web|WebPipeBind|False|
##Examples

###Example 1
    
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 5
    
Add the specified user to the group with Id 5

###Example 2
    
    PS:> Add-SPOUserToGroup -LoginName user@company.com -Identity 'Marketing Site Members'
    

