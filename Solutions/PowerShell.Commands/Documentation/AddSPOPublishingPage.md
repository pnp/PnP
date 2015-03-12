#Add-SPOPublishingPage
*Topic automatically generated on: 2015-03-12*

Adds a publishing page
##Syntax
```powershell
Add-SPOPublishingPage [-Title [<String>]] -PageName [<String>] -PageTemplateName [<String>] [-Publish [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
PageName|String|True|
PageTemplateName|String|True|
Publish|SwitchParameter|False|Publishes the page. Also Approves it if moderation is enabled on the Pages library.
Title|String|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
