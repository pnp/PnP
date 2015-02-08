#Add-SPOPublishingPage
*Topic last generated: 2015-02-08*


##Syntax
    Add-SPOPublishingPage [-Title [<String>]] -PageName [<String>] -PageTemplateName [<String>] [-Publish [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
PageName|String|True|
PageTemplateName|String|True|
Publish|SwitchParameter|False|Publishes the page. Also Approves it if moderation is enabled on the Pages library.
Title|String|False|
Web|WebPipeBind|False|
