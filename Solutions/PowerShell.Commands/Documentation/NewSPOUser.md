#New-SPOUser
*Topic automatically generated on: 2015-06-04*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
```powershell
New-SPOUser -LoginName <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|LoginName|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> New-SPOUser -LogonName user@company.com

<!-- Ref: 4D1D8E3FE3756B14ADFED59FB6FCC7E3 -->