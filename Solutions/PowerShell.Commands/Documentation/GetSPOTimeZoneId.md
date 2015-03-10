#Get-SPOTimeZoneId
*Topic automatically generated on: 2015-03-10*

Adds a SharePoint App to a site
##Syntax
    Get-SPOTimeZoneId [-Match [<String>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Match|String|False|
##Examples

###Example 1
    PS:> Add-SPOnlineApp -Path c:\files\demo.app -Force
This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    

###Example 2
    PS:> Add-SPOnlineApp -Path c:\files\demo.app -LoadOnly
This will load the app in the demo.app package, but will not install it to the site.
 
