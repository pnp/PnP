#Add-SPOView
*Topic last generated: 2015-02-08*


##Syntax
    Add-SPOView -Title [<String>] [-Query [<String>]] -Fields [<String[]>] [-ViewType [<ViewType>]] [-RowLimit [<UInt32>]] [-Personal [<SwitchParameter>]] [-SetAsDefault [<SwitchParameter>]] [-Web [<WebPipeBind>]] [-List [<ListPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Fields|String[]|True|
List|ListPipeBind|False|The ID or Url of the list.
Personal|SwitchParameter|False|
Query|String|False|
RowLimit|UInt32|False|
SetAsDefault|SwitchParameter|False|
Title|String|True|
Web|WebPipeBind|False|
ViewType|ViewType|False|
