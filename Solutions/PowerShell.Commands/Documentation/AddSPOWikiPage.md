#Add-SPOWikiPage
*Topic automatically generated on: 2015-04-28*

Adds a wiki page
##Syntax
```powershell
Add-SPOWikiPage [-Content [<String>]] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Add-SPOWikiPage [-Layout [<WikiPageLayout>]] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Content|String|False|
Layout|WikiPageLayout|False|
ServerRelativePageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
