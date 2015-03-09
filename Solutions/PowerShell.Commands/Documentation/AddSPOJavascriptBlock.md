#Add-SPOJavascriptBlock
*Topic automatically generated on: 2015-03-10*

Adds a link to a JavaScript snippet/block to a web or site collection
##Syntax
    Add-SPOJavascriptBlock -Key [<String>] -Script [<String>] [-SiteScoped [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Key|String|True|
Script|String|True|
SiteScoped|SwitchParameter|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
