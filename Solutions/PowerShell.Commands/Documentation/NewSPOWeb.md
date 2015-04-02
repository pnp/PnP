#New-SPOWeb
*Topic automatically generated on: 2015-04-02*

Creates a new subweb to the current web
##Syntax
```powershell
New-SPOWeb -Title [<String>] -Url [<String>] [-Description [<String>]] [-Locale [<Int32>]] -Template [<String>] [-BreakInheritance [<SwitchParameter>]] [-InheritNavigation [<SwitchParameter>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
BreakInheritance|SwitchParameter|False|By default the subweb will inherit its security from its parent, specify this switch to break this inheritance
Description|String|False|The description of the new web
InheritNavigation|SwitchParameter|False|Specifies whether the site inherits navigation.
Locale|Int32|False|
Template|String|True|The site definition template to use for the new web, e.g. STS#0
Title|String|True|The title of the new web
Url|String|True|The Url of the new web
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> New-SPOWeb -Title "Project A Web" -Url projectA -Description "Information about Project A" -Locale 1033 -Template "STS#0"
Creates a new subweb under the current web with url projectA
