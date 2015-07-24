#Set-SPODocumentSetField
*Topic automatically generated on: 2015-07-21*

Sets a site column from the avaiable content types to a document set
##Syntax
```powershell
Set-SPODocumentSetField -DocumentSet <DocumentSetPipeBind> -Field <FieldPipeBind> [-SetSharedField [<SwitchParameter>]] [-SetWelcomePageField [<SwitchParameter>]] [-RemoveSharedField [<SwitchParameter>]] [-RemoveWelcomePageField [<SwitchParameter>]] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|DocumentSet|DocumentSetPipeBind|True|The document set to set the field in. Either specify a name, a document set template object, an id, or a content type object|
|Field|FieldPipeBind|True|The field to set. The field needs to be available in one of the available content types. Either specify a name, an id or a field object|
|RemoveSharedField|SwitchParameter|False|Removes the field as a Shared Field|
|RemoveWelcomePageField|SwitchParameter|False|Removes the field as a Welcome Page Field|
|SetSharedField|SwitchParameter|False|Set the field as a Shared Field|
|SetWelcomePageField|SwitchParameter|False|Set the field as a Welcome Page field|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Set-SPODocumentSetField -Field "Test Field" -DocumentSet "Test Document Set" -SetAsSharedField -SetAsWelcomePageField
This will set the field, available in one the available content types, as a Shared Field and as a Welcome Page Field.

###Example 2
    PS:> Set-SPODocumentSetField -Field "Test Field" -DocumentSet "Test Document Set" -RemoveAsSharedField -RemoveAsWelcomePageField
This will remove the field, available in one the available content types, as a Shared Field and as a Welcome Page Field.
<!-- Ref: 638931CAA8BD28315003E579B763AC48 -->