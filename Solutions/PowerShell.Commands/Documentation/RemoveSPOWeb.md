#Remove-SPOWeb
*Topic automatically generated on: 2015-07-24*

Removes a subweb in the current web
##Syntax
```powershell
Remove-SPOWeb -Url <String> [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|Url|String|True|The Url of the web|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOWeb -Url projectA
Remove a web
<!-- Ref: C6660CDE893236DADFC3A598B0F6FDCC -->