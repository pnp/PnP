#Resume-SPOWorkflowInstance
*Topic automatically generated on: 2015-08-04*

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
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
