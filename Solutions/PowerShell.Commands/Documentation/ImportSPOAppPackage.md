#Import-SPOAppPackage
*Topic automatically generated on: 2015-03-10*

Adds a SharePoint App to a site
##Syntax
    Import-SPOAppPackage [-Path [<String>]] [-Force [<SwitchParameter>]] [-LoadOnly [<SwitchParameter>]] [-Locale [<Int32>]] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Force|SwitchParameter|False|Will forcibly install the app by activating the app sideloading feature, installing the app, and deactivating the sideloading feature
LoadOnly|SwitchParameter|False|Will only upload the app, but not install it
Locale|Int32|False|Will install the app for the specified locale
Path|String|False|Path pointing to the .app file
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the app in the demo.app package, but will not install it to the site.
 

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    
