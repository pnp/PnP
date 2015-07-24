#Add-SPOWorkflowSubscription
*Topic automatically generated on: 2015-06-11*

Adds a workflow subscription to a list
##Syntax
```powershell
Add-SPOWorkflowSubscription -Name <String> -DefinitionName <String> -List <ListPipeBind> [-StartManually [<SwitchParameter>]] [-StartOnCreated [<SwitchParameter>]] [-StartOnChanged [<SwitchParameter>]] -HistoryListName <String> -TaskListName <String> [-AssociationValues <Dictionary`2>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AssociationValues|Dictionary`2|False||
|DefinitionName|String|True|The name of the workflow definition|
|HistoryListName|String|True||
|List|ListPipeBind|True|The list to add the workflow to|
|Name|String|True|The name of the subscription|
|StartManually|SwitchParameter|False||
|StartOnChanged|SwitchParameter|False||
|StartOnCreated|SwitchParameter|False||
|TaskListName|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: 8155A3EB9A0DDA956896C9EE033836B0 -->