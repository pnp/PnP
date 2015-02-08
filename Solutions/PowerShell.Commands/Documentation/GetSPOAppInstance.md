#Get-SPOAppInstance
*Topic last generated: 2015-02-08*

Returns a SharePoint App Instance
##Syntax
    Get-SPOAppInstance [-Web [<WebPipeBind>]] [-Identity [<GuidPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|False|The Id of the App Instance
Web|WebPipeBind|False|
##Examples

###Example 1
    PS:> Get-SPOAppInstance
This will return all app instances in the site.
 

###Example 2
    PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
This will return an app instance with the specified id.
    
