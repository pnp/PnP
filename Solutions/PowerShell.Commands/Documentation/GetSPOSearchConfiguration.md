#Get-SPOSearchConfiguration
*Topic automatically generated on: 2015-07-08*

Returns the search configuration
##Syntax
```powershell
Get-SPOSearchConfiguration [-Scope <SearchConfigurationScope>] [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Scope|SearchConfigurationScope|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Get-SPOSearchConfiguration
Returns the search configuration for the current web

###Example 2
    PS:> Get-SPOSearchConfiguration -Scope Site
Returns the search configuration for the current site collection
<!-- Ref: 473CD111CD77F01BF3EA5A3E85071DB3 -->