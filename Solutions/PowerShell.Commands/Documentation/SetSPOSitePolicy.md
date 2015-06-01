#Set-SPOSitePolicy
*Topic automatically generated on: 2015-05-28*

Sets a site policy
##Syntax
```powershell
Set-SPOSitePolicy -Name <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|True|The name of the site policy to apply
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Set-SPOSitePolicy -Name "Contoso HBI"
This applies a site policy with the name "Contoso HBI" to the current site. The policy needs to be available in the site.
<!-- Ref: 0190886277D5B860AF10469810E96CFE -->