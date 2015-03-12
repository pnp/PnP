#Add-SPOField
*Topic automatically generated on: 2015-03-12*

Adds a field to a list or as a site column
##Syntax
```powershell
Add-SPOField [-List [<ListPipeBind>]] -DisplayName [<String>] -InternalName [<String>] -Type [<FieldType>] [-Id [<GuidPipeBind>]] [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Add-SPOField -DisplayName [<String>] -InternalName [<String>] -Type [<FieldType>] [-Id [<GuidPipeBind>]] [-Web [<WebPipeBind>]]
```
&nbsp;

```powershell
Add-SPOField [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group [<String>]] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddToDefaultView|SwitchParameter|False|
DisplayName|String|True|
Group|String|False|
Id|GuidPipeBind|False|
InternalName|String|True|
List|ListPipeBind|False|
Required|SwitchParameter|False|
Type|FieldType|True|
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Add-SPOField -List "Demo list" -DisplayName "Location" -InternalName "SPSLocation" -Type Choice -Group "Demo Group" -AddToDefaultView -Choices "Stockholm","Helsinki","Oslo"
This will add field of type Choice to a the list "Demo List".
