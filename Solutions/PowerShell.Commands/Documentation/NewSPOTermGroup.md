#New-SPOTermGroup
*Topic automatically generated on: 2015-06-11*

Creates a taxonomy term group
##Syntax
```powershell
New-SPOTermGroup -GroupName <String> [-GroupId <Guid>] [-Description <String>] [-TermStoreName <String>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Description|String|False|Description to use for the term group.|
|GroupId|Guid|False|GUID to use for the term group; if not specified, or the empty GUID, a random GUID is generated and used.|
|GroupName|String|True|Name of the taxonomy term group to create.|
|TermStoreName|String|False|Term store to check; if not specified the default term store is used.|
<!-- Ref: AB4D0A2FE3E9B05E19184AA5470B29DD -->