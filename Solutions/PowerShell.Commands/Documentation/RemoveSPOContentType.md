#Remove-SPOContentType
*Topic last generated: 2015-02-08*

Removes a content type
##Syntax
    Remove-SPOContentType [-Force [<SwitchParameter>]] [-Web [<WebPipeBind>]] -Identity [<ContentTypePipeBind>]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|
Identity|ContentTypePipeBind|True|The name or ID of the content type to remove
Web|WebPipeBind|False|
##Examples

###Example 1
    PS:> Remove-SPOContentType -Identity "Project Document"

