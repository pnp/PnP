#Get-SPOAppInstance
*Topic automatically generated on: 2015-04-28*

Returns a SharePoint App Instance
##Syntax
```powershell
Get-SPOAppInstance [-Web [<WebPipeBind>]] [-Identity [<GuidPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|False|The Id of the App Instance
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOAppInstance
This will return all app instances in the site.
 

###Example 2
    PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
This will return an app instance with the specified id.
    
