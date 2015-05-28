#Get-SPOAppInstance
*Topic automatically generated on: 2015-05-28*

Returns a SharePoint AddIn Instance
##Syntax
```powershell
Get-SPOAppInstance [-Web <WebPipeBind>] [-Identity <GuidPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|GuidPipeBind|False|The Id of the App Instance
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Get-SPOAppInstance
This will return all addin instances in the site.
 

###Example 2
    PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
This will return an addin instance with the specified id.
    
<!-- Ref: AAF7EDF69651276D31B75991A0ECBAF5 -->