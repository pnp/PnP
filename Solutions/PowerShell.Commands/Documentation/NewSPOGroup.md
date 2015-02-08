#New-SPOGroup
*Topic last generated: 2015-02-08*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
    New-SPOGroup -Title [<String>] [-Description [<String>]] [-Owner [<String>]] [-AllowRequestToJoinLeave [<SwitchParameter>]] [-AutoAcceptRequestToJoinLeave [<SwitchParameter>]] [-SetAssociatedGroup [<AssociatedGroupType>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AllowRequestToJoinLeave|SwitchParameter|False|
AutoAcceptRequestToJoinLeave|SwitchParameter|False|
Description|String|False|
Owner|String|False|
SetAssociatedGroup|AssociatedGroupType|False|
Title|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> New-SPOUser -LogonName user@company.com


