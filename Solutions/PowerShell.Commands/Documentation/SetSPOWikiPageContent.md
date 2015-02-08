#Set-SPOWikiPageContent
*Topic automatically generated on: 2015-02-08*


##Syntax
    Set-SPOWikiPageContent -Content [<String>] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]

&nbsp;

    Set-SPOWikiPageContent -Path [<String>] -ServerRelativePageUrl [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Content|String|True|
Path|String|True|
ServerRelativePageUrl|String|True|Site Relative Page Url
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
