#Remove-SPOView
*Topic automatically generated on: 2015-06-11*

Deletes a view from a list
##Syntax
```powershell
Remove-SPOView [-Force [<SwitchParameter>]] [-Web <WebPipeBind>] -Identity <ViewPipeBind> -List <ListPipeBind>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False||
|Identity|ViewPipeBind|True|The ID or Title of the list.|
|List|ListPipeBind|True|The ID or Url of the list.|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: 08B19F7A713F8149EF854639ED5A1893 -->