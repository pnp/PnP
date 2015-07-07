#Add-SPOField
*Topic automatically generated on: 2015-07-01*

Adds a field to a list or as a site column
##Syntax
```powershell
Add-SPOField -List <ListPipeBind> -Field <FieldPipeBind> [-Web <WebPipeBind>]
```


```powershell
Add-SPOField [-List <ListPipeBind>] -DisplayName <String> -InternalName <String> -Type <FieldType> [-Id <GuidPipeBind>] [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group <String>] [-Web <WebPipeBind>]
```


```powershell
Add-SPOField [-AddToDefaultView [<SwitchParameter>]] [-Required [<SwitchParameter>]] [-Group <String>] [-Web <WebPipeBind>]
```


```powershell
Add-SPOField -DisplayName <String> -InternalName <String> -Type <FieldType> [-Id <GuidPipeBind>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AddToDefaultView|SwitchParameter|False||
|DisplayName|String|True||
|Field|FieldPipeBind|True||
|Group|String|False||
|Id|GuidPipeBind|False||
|InternalName|String|True||
|List|ListPipeBind|True||
|Required|SwitchParameter|False||
|Type|FieldType|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOField -List "Demo list" -DisplayName "Location" -InternalName "SPSLocation" -Type Choice -Group "Demo Group" -AddToDefaultView -Choices "Stockholm","Helsinki","Oslo"
This will add field of type Choice to a the list "Demo List".

###Example 2
    PS:>Add-SPOField -List "Demo list" -DisplayName "Speakers" -InternalName "SPSSpeakers" -Type MultiChoice -Group "Demo Group" -AddToDefaultView -Choices "Obiwan Kenobi","Darth Vader", "Anakin Skywalker"
This will add field of type Multiple Choice to a the list "Demo List". (you can pick several choices for the same item)
<!-- Ref: D6F4CA632CF71363438BD82423CAD561 -->