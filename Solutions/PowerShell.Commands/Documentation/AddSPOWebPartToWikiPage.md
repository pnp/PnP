#Add-SPOWebPartToWikiPage
*Topic automatically generated on: 2015-06-03*

Adds a webpart to a wiki page in a specified table row and column
##Syntax
```powershell
Add-SPOWebPartToWikiPage -Path <String> -PageUrl <String> -Row <Int32> -Column <Int32> [-AddSpace [<SwitchParameter>]] [-Web <WebPipeBind>]
```


```powershell
Add-SPOWebPartToWikiPage -Xml <String> -PageUrl <String> -Row <Int32> -Column <Int32> [-AddSpace [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AddSpace|SwitchParameter|False||
|Column|Int32|True||
|PageUrl|String|True||
|Path|String|True||
|Row|Int32|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
|Xml|String|True||
<!-- Ref: 708507A6170796ACB8BF8043AB27C6FB -->