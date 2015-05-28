#Import-SPOAppPackage
*Topic automatically generated on: 2015-05-28*

<<<<<<< HEAD
Adds a SharePoint Addin to a site
=======
Adds a SharePoint add-in to a site
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
##Syntax
```powershell
Import-SPOAppPackage -Path <String> [-Force [<SwitchParameter>]] [-LoadOnly [<SwitchParameter>]] [-Locale <Int32>] [-Web <WebPipeBind>]```
&nbsp;

##Detailed Description
<<<<<<< HEAD
This commands requires that you have an addin package to deploy
=======
This commands requires that you have an add-in package to deploy
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
<<<<<<< HEAD
Force|SwitchParameter|False|Will forcibly install the app by activating the addin sideloading feature, installing the addin, and deactivating the sideloading feature
LoadOnly|SwitchParameter|False|Will only upload the addin, but not install it
Locale|Int32|False|Will install the addin for the specified locale
=======
Force|SwitchParameter|False|Will forcibly install the add-in by activating the add-in sideloading feature, installing the add-in, and deactivating the sideloading feature
LoadOnly|SwitchParameter|False|Will only upload the add-in, but not install it
Locale|Int32|False|Will install the add-in for the specified locale
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
Path|String|True|Path pointing to the .app file
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
<<<<<<< HEAD
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the addin in the demo.app package, but will not install it to the site.
=======
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the add-in sideloading feature, upload and install the add-in, and deactivate the add-in sideloading feature.
    

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the add-in in the demo.app package, but will not install it to the site.
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
 

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the addin sideloading feature, upload and install the addin, and deactivate the addin sideloading feature.
    
<!-- Ref: DE8A10673DA57613096A614DECF76083 -->
