#Import-SPOAppPackage
*Topic automatically generated on: 2015-05-04*

Adds a SharePoint App to a site
##Syntax
```powershell
Import-SPOAppPackage -Path <String> [-Force [<SwitchParameter>]] [-LoadOnly [<SwitchParameter>]] [-Locale <Int32>] [-Web <WebPipeBind>]```
&nbsp;

##Detailed Description
This commands requires that you have an app package to deploy

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Will forcibly install the app by activating the app sideloading feature, installing the app, and deactivating the sideloading feature
LoadOnly|SwitchParameter|False|Will only upload the app, but not install it
Locale|Int32|False|Will install the app for the specified locale
Path|String|True|Path pointing to the .app file
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the app in the demo.app package, but will not install it to the site.
 
<!-- Ref: 09B9495CF08660CC96FC2D6E7A5F5CBC -->