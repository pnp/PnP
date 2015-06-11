#Apply-SPOSitePolicy
*Topic automatically generated on: 2015-05-27*

Sets a site policy
##Syntax
```powershell
Apply-SPOSitePolicy -Name <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|True|The name of the site policy to apply
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Apply-SPOSitePolicy -Name "Contoso HBI"
This apply a site policy with the name "Contoso HBI" to the current site.
<!-- Ref: D73F87889866C0C7013232D288CD2CC3 -->