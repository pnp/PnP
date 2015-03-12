#Add&#8209;SPOWikiPage
*Topic automatically generated on: 2015-03-12*

Adds a wiki page
##Syntax
```powershell
Add&#8209;SPOWikiPage [-Content [<String>]] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Add&#8209;SPOWikiPage [-Layout [<WikiPageLayout>]] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Content|String|False|
Layout|WikiPageLayout|False|
ServerRelativePageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
