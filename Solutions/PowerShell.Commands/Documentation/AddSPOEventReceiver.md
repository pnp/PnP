#Add-SPOEventReceiver
*Topic automatically generated on: 2015-06-03*

Adds a new event receiver
##Syntax
```powershell
Add-SPOEventReceiver -List <ListPipeBind> -Name <String> -Url <String> -EventReceiverType <EventReceiverType> -Synchronization <EventReceiverSynchronization> [-SequenceNumber <Int32>] [-Force [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|EventReceiverType|EventReceiverType|True||
|Force|SwitchParameter|False||
|List|ListPipeBind|True||
|Name|String|True||
|SequenceNumber|Int32|False||
|Synchronization|EventReceiverSynchronization|True||
|Url|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOEventReceiver -List "ProjectList" -Name "TestEventReceiver" -Url https://yourserver.azurewebsites.net/eventreceiver.svc -EventReceiverType ItemAdded -Synchronization Asynchronous
This will add a new event receiver that is executed after an item has been added to the ProjectList list
<!-- Ref: D9FA92606FB213730776E3063ACEB3D0 -->