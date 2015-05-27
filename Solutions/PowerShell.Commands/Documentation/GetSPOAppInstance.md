#Get-SPOAppInstance
*Topic automatically generated on: 2015-04-29*

Returns a SharePoint add-in Instance
##Syntax
```powershell
Get-SPOAppInstance [-Web <WebPipeBind>] [-Identity <GuidPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|False|The Id of the add-in Instance
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Get-SPOAppInstance
This will return all add-in instances in the site.
 

###Example 2
    PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
This will return an add-in instance with the specified id.
    
<!-- Ref: 65AC11191269C133729C6878608539B8 -->