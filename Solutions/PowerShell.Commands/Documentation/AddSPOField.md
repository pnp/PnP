#Add-SPOField
*Topic automatically generated on: 2015-06-01*

Adds a field to a list or as a site column
##Syntax
```powershell
Add-SPOField -List <ListPipeBind> -Field <FieldPipeBind> [-Web <WebPipeBind>]```
&nbsp;

```powershell
Add-SPOField [-List <ListPipeBind>] -DisplayName <String> -InternalName <String> -Type <FieldType> [-Id <GuidPipeBind>] [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group <String>] [-Web <WebPipeBind>]```
&nbsp;

```powershell
Add-SPOField -DisplayName <String> -InternalName <String> -Type <FieldType> [-Id <GuidPipeBind>] [-Web <WebPipeBind>]```
&nbsp;

```powershell
Add-SPOField [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group <String>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
AddToDefaultView|SwitchParameter|False|
DisplayName|String|True|
Field|FieldPipeBind|True|
Group|String|False|
Id|GuidPipeBind|False|
InternalName|String|True|
List|ListPipeBind|True|
Required|SwitchParameter|False|
Type|FieldType|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Add-SPOField -List "Demo list" -DisplayName "Location" -InternalName "SPSLocation" -Type Choice -Group "Demo Group" -AddToDefaultView -Choices "Stockholm","Helsinki","Oslo"
This will add field of type Choice to a the list "Demo List".
<!-- Ref: CEBCDC17E0F9B33558D7A95FCD1BE221 -->