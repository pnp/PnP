#Set&#8209;SPOWikiPageContent
*Topic automatically generated on: 2015-03-12*

Sets the contents of a wikipage
##Syntax
```powershell
Set&#8209;SPOWikiPageContent -Content [<String>] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Set&#8209;SPOWikiPageContent -Path [<String>] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Content|String|True|
Path|String|True|
ServerRelativePageUrl|String|True|Site Relative Page Url
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
