#Remove-SPOWorkflowSubscription
*Topic automatically generated on: 2015-04-28*

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
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
