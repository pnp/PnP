#Set-SPOMasterPage
*Topic automatically generated on: 2015-04-29*

Sets the default master page of the current web.
##Syntax
```powershell
Set-SPOMasterPage [-MasterPageUrl <String>] [-CustomMasterPageUrl <String>] [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
CustomMasterPageUrl|String|False|
MasterPageUrl|String|False|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
    PS:> Set-SPOMasterPage -MasterPageUrl /sites/projects/_catalogs/masterpage/oslo.master


<!-- Ref: 3A245971B5F751DE205C523744A4A0AA -->