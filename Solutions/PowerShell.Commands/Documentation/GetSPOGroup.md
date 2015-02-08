#Get-SPOGroup
*Topic automatically generated on: 2015-02-08*

Returns a specific group or all groups.
##Syntax
    Get-SPOGroup [-Web [<WebPipeBind>]] [-Identity [<GroupPipeBind>]]

&nbsp;

    Get-SPOGroup [-AssociatedMemberGroup [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

    Get-SPOGroup [-AssociatedVisitorGroup [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

    Get-SPOGroup [-AssociatedOwnerGroup [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AssociatedMemberGroup|SwitchParameter|False|Retrieve the associated member group
AssociatedOwnerGroup|SwitchParameter|False|Retrieve the associated owner group
AssociatedVisitorGroup|SwitchParameter|False|Retrieve the associated visitor group
Identity|GroupPipeBind|False|Get a specific group by name
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> Get-SPOGroup



###Example 2
    
PS:> Get-SPOGroup -Name 'Site Members'


