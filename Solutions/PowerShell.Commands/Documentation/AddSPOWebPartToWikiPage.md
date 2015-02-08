#Add-SPOWebPartToWikiPage
*Topic last generated: 2015-02-08*


##Syntax
    Add-SPOWebPartToWikiPage -Xml [<String>] -PageUrl [<String>] -Row [<Int32>] -Column [<Int32>] [-AddSpace [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

    Add-SPOWebPartToWikiPage -Path [<String>] -PageUrl [<String>] -Row [<Int32>] -Column [<Int32>] [-AddSpace [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddSpace|SwitchParameter|False|
Column|Int32|True|
PageUrl|String|True|
Path|String|True|
Row|Int32|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
Xml|String|True|
