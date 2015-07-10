#New-SPOGroup
*Topic automatically generated on: 2015-06-11*

Adds a user to the build-in Site User Info List and returns a user object
##Syntax
```powershell
New-SPOGroup -Title <String> [-Description <String>] [-Owner <String>] [-AllowRequestToJoinLeave [<SwitchParameter>]] [-AutoAcceptRequestToJoinLeave [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AllowRequestToJoinLeave|SwitchParameter|False||
|AutoAcceptRequestToJoinLeave|SwitchParameter|False||
|Description|String|False||
|Owner|String|False||
|Title|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> New-SPOUser -LogonName user@company.com

<!-- Ref: FFFFA1246201B3EA819C03275D418E9C -->