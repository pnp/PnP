#Remove-SPOWebPart
*Topic last generated: 2015-02-08*


##Syntax
    Remove-SPOWebPart -Identity [<GuidPipeBind>] -PageUrl [<String>] [-Web [<WebPipeBind>]]

&nbsp;

    Remove-SPOWebPart -Name [<String>] -PageUrl [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Name|String|True|
PageUrl|String|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
