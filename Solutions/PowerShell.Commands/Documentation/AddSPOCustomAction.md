#Add-SPOCustomAction
*Topic automatically generated on: 2015-02-08*

Adds a custom action to a web
##Syntax
    Add-SPOCustomAction -Title [<String>] -Description [<String>] -Group [<String>] -Location [<String>] -Sequence [<Int32>] -Url [<String>] [-Rights [<List`1>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Description|String|True|
Group|String|True|
Location|String|True|
Rights|List`1|False|
Sequence|Int32|True|
Title|String|True|
Url|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
