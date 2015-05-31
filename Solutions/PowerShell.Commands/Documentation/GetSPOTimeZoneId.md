#Get-SPOTimeZoneId
*Topic automatically generated on: 2015-05-15*

Returns a time zone ID
##Syntax
```powershell
Get-SPOTimeZoneId [-Match <String>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Match|String|False|
##Examples

###Example 1
    PS:> Get-SPOTimeZoneId -Match Stockholm
This will return the time zone IDs for Stockholm
    

###Example 2
    PS:> Get-SPOTimeZoneId
This will return all time zone IDs in use by Office 365.
 
<!-- Ref: 2CACB67A0EF5C619AF611EBC641B9E8A -->