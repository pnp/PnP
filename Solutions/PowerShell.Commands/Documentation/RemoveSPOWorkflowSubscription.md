#Remove-SPOWorkflowSubscription
*Topic automatically generated on: 2015-08-04*

Removes a workflow subscription
##Syntax
```powershell
Remove-SPOWorkflowSubscription [-Web [<WebPipeBind>]] -Identity [<WorkflowSubscriptionPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|WorkflowSubscriptionPipeBind|True|The subscription to remove
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
