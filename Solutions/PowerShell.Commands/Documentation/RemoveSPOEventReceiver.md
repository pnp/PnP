#Remove-SPOEventReceiver
*Topic automatically generated on: 2015-06-03*

Removes/unregisters a specific event receiver
##Syntax
```powershell
Remove-SPOEventReceiver [-List <ListPipeBind>] -Identity <GuidPipeBind> [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|Identity|GuidPipeBind|True||
|List|ListPipeBind|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the current web

###Example 2
    PS:> Remove-SPOEventReceiver -List ProjectList -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the list with name "ProjectList"
<!-- Ref: A75AD55B95FAD44D844E56D58E98AF89 -->