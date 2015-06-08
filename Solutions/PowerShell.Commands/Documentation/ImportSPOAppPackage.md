#Import-SPOAppPackage
*Topic automatically generated on: 2015-06-03*

Adds a SharePoint Addin to a site
##Syntax
```powershell
Import-SPOAppPackage -Path <String> [-Force [<SwitchParameter>]] [-LoadOnly [<SwitchParameter>]] [-Locale <Int32>] [-Web <WebPipeBind>]
```


##Detailed Description
This commands requires that you have an addin package to deploy

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Force|SwitchParameter|False|Will forcibly install the app by activating the addin sideloading feature, installing the addin, and deactivating the sideloading feature|
|LoadOnly|SwitchParameter|False|Will only upload the addin, but not install it|
|Locale|Int32|False|Will install the addin for the specified locale|
|Path|String|True|Path pointing to the .app file|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -LoadOnly
This will load the addin in the demo.app package, but will not install it to the site.
 

###Example 2
    PS:> Import-SPOAppPackage -Path c:\files\demo.app -Force
This load first activate the addin sideloading feature, upload and install the addin, and deactivate the addin sideloading feature.
    
<!-- Ref: 8C94D6F50F27D77EF509C8FDB6C51887 -->