#Remove-SPOEventReceiver
*Topic automatically generated on: 2015-05-04*

Removes/unregisters a specific event receiver
##Syntax
```powershell
Remove-SPOEventReceiver [-List <ListPipeBind>] -Identity <GuidPipeBind> [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|GuidPipeBind|True|
List|ListPipeBind|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Remove-SPOEventReceiver -List ProjectList -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the list with name "ProjectList"

###Example 2
    PS:> Remove-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22
This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the current web
<!-- Ref: 81E46E20CE427FA35FE0A92152709F5D -->