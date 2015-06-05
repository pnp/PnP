#Add-SPOView
*Topic automatically generated on: 2015-06-05*

Adds a view to a list
##Syntax
```powershell
Add-SPOView -Title <String> [-Query <String>] -Fields <String[]> [-ViewType <ViewType>] [-RowLimit <UInt32>] [-Personal [<SwitchParameter>]] [-SetAsDefault [<SwitchParameter>]] [-Web <WebPipeBind>] [-List <ListPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Fields|String[]|True||
|List|ListPipeBind|False|The ID or Url of the list.|
|Personal|SwitchParameter|False||
|Query|String|False||
|RowLimit|UInt32|False||
|SetAsDefault|SwitchParameter|False||
|Title|String|True||
|ViewType|ViewType|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    Add-SPOView -List "Demo List" -Title "Demo View" -Fields "Title","Address"

<!-- Ref: A1C02537654D1CD76C600B1B8A9A7B7B -->