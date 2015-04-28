#Add-SPONavigationNode
*Topic automatically generated on: 2015-04-28*

Adds a menu item to either the quicklaunch or top navigation
##Syntax
```powershell
Add-SPONavigationNode -Location [<NavigationType>] -Title [<String>] [-Url [<String>]] [-Header [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Header|String|False|
Location|NavigationType|True|
Title|String|True|
Url|String|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
