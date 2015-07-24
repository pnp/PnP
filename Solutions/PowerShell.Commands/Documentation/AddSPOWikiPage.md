#Add-SPOWikiPage
*Topic automatically generated on: 2015-06-11*

Adds a wiki page
##Syntax
```powershell
Add-SPOWikiPage [-Content <String>] -ServerRelativePageUrl <String> [-Web <WebPipeBind>]
```


```powershell
Add-SPOWikiPage [-Layout <WikiPageLayout>] -ServerRelativePageUrl <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Content|String|False||
|Layout|WikiPageLayout|False||
|ServerRelativePageUrl|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: FFB6EE9086AC968F9A741618CBBB4E0F -->