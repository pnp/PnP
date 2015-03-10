#Add-SPOJavascriptLink
*Topic automatically generated on: 2015-03-10*

Adds a link to a JavaScript file to a web or sitecollection
##Syntax
    Add-SPOJavascriptLink -Key [<String>] -Url [<String[]>] [-SiteScoped [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Key|String|True|
SiteScoped|SwitchParameter|False|
Url|String[]|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
