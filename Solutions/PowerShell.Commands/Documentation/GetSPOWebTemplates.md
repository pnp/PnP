#Get-SPOWebTemplates
*Topic automatically generated on: 2015-06-03*

Office365 only: Returns the available web templates.
##Syntax
```powershell
Get-SPOWebTemplates [-Lcid <UInt32>] [-CompatibilityLevel <Int32>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|CompatibilityLevel|Int32|False||
|Lcid|UInt32|False||
##Examples

###Example 1
    PS:> Get-SPOWebTemplates


###Example 2
    PS:> Get-SPOWebTemplates -LCID 1033
Returns all webtemplates for the Locale with ID 1033 (English)
<!-- Ref: 0D1E6E1A845F9A011E9D30F678C66D33 -->