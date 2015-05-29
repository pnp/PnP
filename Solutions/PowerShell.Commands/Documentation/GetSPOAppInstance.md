#Get-SPOAppInstance
*Topic automatically generated on: 2015-05-28*

<<<<<<< HEAD
Returns a SharePoint AddIn Instance
=======
Returns a SharePoint add-in Instance
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
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
<<<<<<< HEAD
This will return all addin instances in the site.
=======
This will return all add-in instances in the site.
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
 

###Example 2
    PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
<<<<<<< HEAD
This will return an addin instance with the specified id.
=======
This will return an add-in instance with the specified id.
>>>>>>> 80f41dceaa3bcd5c3eb44a5dfcc3e3a4908809ab
    
<!-- Ref: AAF7EDF69651276D31B75991A0ECBAF5 -->
