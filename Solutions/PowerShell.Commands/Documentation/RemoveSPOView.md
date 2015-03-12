#Remove&#8209;SPOView
*Topic automatically generated on: 2015-03-12*

Deletes a view from a list
##Syntax
```powershell
Remove&#8209;SPOView [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]] -Identity [<ViewPipeBind>] -List [<ListPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|ViewPipeBind|True|The ID or Title of the list.
List|ListPipeBind|True|The ID or Url of the list.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
