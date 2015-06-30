#Set-SPOSitePolicy
*Topic automatically generated on: 2015-06-03*

Sets a site policy
##Syntax
```powershell
Set-SPOSitePolicy -Name <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Name|String|True|The name of the site policy to apply|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Set-SPOSitePolicy -Name "Contoso HBI"
This applies a site policy with the name "Contoso HBI" to the current site. The policy needs to be available in the site.
<!-- Ref: 147B898C7DBB475910D0C13F064D069E -->