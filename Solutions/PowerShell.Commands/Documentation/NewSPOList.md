#New-SPOList
*Topic last generated: 2015-02-08*

Creates a new list
##Syntax
    New-SPOList -Title [<String>] -Template [<ListTemplateType>] [-Url [<String>]] [-EnableVersioning [<SwitchParameter>]] [-QuickLaunchOptions [<QuickLaunchOptions>]] [-EnableContentTypes [<SwitchParameter>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
EnableContentTypes|SwitchParameter|False|
EnableVersioning|SwitchParameter|False|
QuickLaunchOptions|QuickLaunchOptions|False|Obsolete
Template|ListTemplateType|True|The type of list to create.
Title|String|True|
Url|String|False|If set, will override the url of the list.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> New-SPOList -Title Announcements -Template Announcements


###Example 2
    PS:> New-SPOList -Title "Demo List" -Url "DemoList" -Template Announcements
Create a list with a title that is different from the url
