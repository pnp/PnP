#Get-SPOTimeZoneId
*Topic automatically generated on: 2015-06-03*

Returns a time zone ID
##Syntax
```powershell
Get-SPOTimeZoneId [-Match <String>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Match|String|False||
##Examples

###Example 1
    PS:> Get-SPOTimeZoneId
This will return all time zone IDs in use by Office 365.
 

###Example 2
    PS:> Get-SPOTimeZoneId -Match Stockholm
This will return the time zone IDs for Stockholm
    
<!-- Ref: 5F689A30354CFD1105A857BA9A1CBF1B -->