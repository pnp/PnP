#Add-SPONavigationNode
*Topic automatically generated on: 2015-06-03*

Adds a menu item to either the quicklaunch or top navigation
##Syntax
```powershell
Add-SPONavigationNode -Location <NavigationType> -Title <String> [-Url <String>] [-Header <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Header|String|False||
|Location|NavigationType|True||
|Title|String|True||
|Url|String|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: BA10CA51DE2C046F03670127CBB5F1A7 -->