#Remove-SPOView
*Topic automatically generated on: 2015-04-29*

Deletes a view from a list
##Syntax
```powershell
Remove-SPOView [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -Identity <ViewPipeBind> -List <ListPipeBind>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|ViewPipeBind|True|The ID or Title of the list.
List|ListPipeBind|True|The ID or Url of the list.
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: B88C210D159D08842F6D2C8EB49A8F8A -->