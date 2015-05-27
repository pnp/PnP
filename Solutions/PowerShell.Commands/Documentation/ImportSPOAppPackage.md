#Import-SPOAppPackage
*Topic automatically generated on: 2015-05-04*

Adds a SharePoint add-in to a site
##Syntax
```powershell
Import-SPOAppPackage -Path <String> [-Force [<SwitchParameter>]] [-LoadOnly [<SwitchParameter>]] [-Locale <Int32>] [-Web <WebPipeBind>]```
&nbsp;

##Detailed Description
This commands requires that you have an add-in package to deploy

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Will forcibly install the add-in by activating the add-in sideloading feature, installing the add-in, and deactivating the sideloading feature
LoadOnly|SwitchParameter|False|Will only upload the add-in, but not install it
Locale|Int32|False|Will install the add-in for the specified locale
Path|String|True|Path pointing to the .app file
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the add-in sideloading feature, upload and install the add-in, and deactivate the add-in sideloading feature.
    

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the add-in in the demo.app package, but will not install it to the site.
 
<!-- Ref: 09B9495CF08660CC96FC2D6E7A5F5CBC -->