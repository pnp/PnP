#Get-SPOView
*Topic automatically generated on: 2015-03-12*

Returns one or all views from a list
##Syntax
```powershell
Get-SPOView [-Identity [<ViewPipeBind>]] [-Web [<WebPipeBind>]] -List [<ListPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|ViewPipeBind|False|
List|ListPipeBind|True|The ID or Url of the list.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
