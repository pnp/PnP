#Add-SPOWorkflowDefinition
*Topic automatically generated on: 2015-06-11*

Adds a workflow definition
##Syntax
```powershell
Add-SPOWorkflowDefinition -Definition <WorkflowDefinition> [-DoNotPublish [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Definition|WorkflowDefinition|True|The workflow definition to add.|
|DoNotPublish|SwitchParameter|False|By default workflow definitions will be publish, specify this switch to override that.|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    Add-SPOWorkflowDefinition -Definition $wfdef
Adds an existing workflow definition, retrieved by Get-SPOWorkflowDefinition, to a site.
<!-- Ref: 829C93535F592E7303A24B62662CA574 -->