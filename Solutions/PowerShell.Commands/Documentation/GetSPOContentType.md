#Get-SPOContentType
*Topic automatically generated on: 2015-03-10*

Retrieves a content type
##Syntax
    Get-SPOContentType [-Web [<WebPipeBind>]] [-Identity [<ContentTypePipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|ContentTypePipeBind|False|Name or ID of the content type to retrieve
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOContentType -Identity "Project Document"
This will add an existing content type to a list and sets it as the default content type
