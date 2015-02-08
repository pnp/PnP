#Add-SPOEventReceiver
*Topic last generated: 2015-02-08*

Adds a new event receiver
##Syntax
    Add-SPOEventReceiver -List [<ListPipeBind>] -Name [<String>] -Url [<String>] -EventReceiverType [<EventReceiverType>] -Synchronization [<EventReceiverSynchronization>] [-SequenceNumber [<Int32>]] [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
EventReceiverType|EventReceiverType|True|
Force|SwitchParameter|False|
List|ListPipeBind|True|
Name|String|True|
SequenceNumber|Int32|False|
Synchronization|EventReceiverSynchronization|True|
Url|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Add-SPOEventReceiver -List "ProjectList" -Name "TestEventReceiver" -Url https://yourserver.azurewebsites.net/eventreceiver.svc -EventReceiverType ItemAdded -Synchronization Asynchronous
This will add a new event receiver that is executed after an item has been added to the ProjectList list
