#New-SPOUser
*Topic automatically generated on: 2015-03-12*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
```powershell
New-SPOUser -LoginName [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
LoginName|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> New-SPOUser -LogonName user@company.com


