#New-SPOUser
*Topic last generated: 2015-02-08*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
    New-SPOUser -LoginName [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
LoginName|String|True|
Web|WebPipeBind|False|
##Examples

###Example 1
    
PS:> New-SPOUser -LogonName user@company.com


