#New-SPOList
*Topic automatically generated on: 2015-07-08*

Creates a new list
##Syntax
```powershell
New-SPOList -Title <String> -Template <ListTemplateType> [-Url <String>] [-EnableVersioning [<SwitchParameter>]] [-EnableContentTypes [<SwitchParameter>]] [-OnQuickLaunch [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|EnableContentTypes|SwitchParameter|False||
|EnableVersioning|SwitchParameter|False||
|OnQuickLaunch|SwitchParameter|False||
|Template|ListTemplateType|True|The type of list to create.|
|Title|String|True||
|Url|String|False|If set, will override the url of the list.|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> New-SPOList -Title Announcements -Template Announcements


###Example 2
    PS:> New-SPOList -Title "Demo List" -Url "DemoList" -Template Announcements
Create a list with a title that is different from the url
<!-- Ref: B153A64AED78F846D46BC64645C6ACE1 -->