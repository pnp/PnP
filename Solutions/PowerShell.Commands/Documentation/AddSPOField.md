#Add-SPOField
*Topic automatically generated on: 2015-02-08*


##Syntax
    Add-SPOField [-List [<ListPipeBind>]] -DisplayName [<String>] -InternalName [<String>] -Type [<FieldType>] [-Id [<GuidPipeBind>]] [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group [<String>]] [-FieldOptions [<AddFieldOptions>]] [-Web [<WebPipeBind>]]

&nbsp;

    Add-SPOField -DisplayName [<String>] -InternalName [<String>] -Type [<FieldType>] [-Id [<GuidPipeBind>]] [-FieldOptions [<AddFieldOptions>]] [-Web [<WebPipeBind>]]

&nbsp;

    Add-SPOField [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group [<String>]] [-FieldOptions [<AddFieldOptions>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddToDefaultView|SwitchParameter|False|
DisplayName|String|True|
FieldOptions|AddFieldOptions|False|
Group|String|False|
Id|GuidPipeBind|False|
InternalName|String|True|
List|ListPipeBind|False|
Required|SwitchParameter|False|
Type|FieldType|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
