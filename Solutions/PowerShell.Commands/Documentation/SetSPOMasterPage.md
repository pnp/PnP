#Set-SPOMasterPage
*Topic automatically generated on: 2015-06-11*

Sets the default master page of the current web.
##Syntax
```powershell
Set-SPOMasterPage [-MasterPageUrl <String>] [-CustomMasterPageUrl <String>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|CustomMasterPageUrl|String|False||
|MasterPageUrl|String|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    
    PS:> Set-SPOMasterPage -MasterPageUrl /sites/projects/_catalogs/masterpage/oslo.master


<!-- Ref: EF6353A4531291AC88F4ACB0AC487B00 -->