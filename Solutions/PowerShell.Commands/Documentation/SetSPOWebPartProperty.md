#Set-SPOWebPartProperty
*Topic last generated: 2015-02-08*


##Syntax
    Set-SPOWebPartProperty -PageUrl [<String>] -Identity [<GuidPipeBind>] -Key [<String>] -Value [<Object>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|True|
Key|String|True|
PageUrl|String|True|
Value|Object|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
