#Set-SPOHomePage
*Topic automatically generated on: 2015-04-29*

Sets the home page of the current web.
##Syntax
```powershell
Set-SPOHomePage [-Web <WebPipeBind>] -Path <String>```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Path|String|True|The root folder relative path of the homepage
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
    PS:> Set-SPOHomePage -Path SitePages/Home.aspx

Sets the home page to the home.aspx file which resides in the SitePages library
<!-- Ref: 902455220D241402B61878695C6CCEDE -->