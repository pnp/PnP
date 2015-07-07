#Set-SPOHomePage
*Topic automatically generated on: 2015-06-11*

Sets the home page of the current web.
##Syntax
```powershell
Set-SPOHomePage [-Web <WebPipeBind>] -Path <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Path|String|True|The root folder relative path of the homepage|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    
    PS:> Set-SPOHomePage -Path SitePages/Home.aspx

Sets the home page to the home.aspx file which resides in the SitePages library
<!-- Ref: 20BA843BB4A4A363DC1CD5F208B26560 -->