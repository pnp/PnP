#Add-SPOContentType
*Topic automatically generated on: 2015-03-11*

Adds a new content type
##Syntax
    Add-SPOContentType -Name [<String>] [-ContentTypeId [<String>]] [-Description [<String>]] [-Group [<String>]] [-ParentContentType [<ContentType>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
ContentTypeId|String|False|If specified, in the format of 0x0100233af432334r434343f32f3, will create a content type with the specific ID
Description|String|False|
Group|String|False|
Name|String|True|
ParentContentType|ContentType|False|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Add-SPOContentType -Name "Project Document" -Description "Use for Contoso projects" -Group "Contoso Content Types" -ParentContentType $ct
This will add a new content type based on the parent content type stored in the $ct variable.
