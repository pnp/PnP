#Resume-SPOWorkflowInstance
*Topic automatically generated on: 2015-03-12*

Resumes a previously stopped workflow instance
##Syntax
```powershell
Resume-SPOWorkflowInstance [-Web [<WebPipeBind>]] -Identity [<WorkflowInstancePipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|WorkflowInstancePipeBind|True|The instance to resume
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
