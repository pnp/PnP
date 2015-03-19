#Add-SPONavigationNode
*Topic automatically generated on: 2015-02-08*


##Syntax
    Add-SPONavigationNode -Location [<NavigationNodeType>] -Title [<String>] [-Url [<String>]] [-Header [<String>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Header|String|False|
Location|NavigationNodeType|True|Either 'Top' or 'Quicklaunch'
Title|String|True|
Url|String|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
