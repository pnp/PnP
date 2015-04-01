#New-SPOTaxonomyTermGroup
*Topic automatically generated on: 2015-04-01*

Creates a taxonomy term group
##Syntax
```powershell
New-SPOTaxonomyTermGroup -GroupName <String> [-GroupId <Guid>] [-Description <String>] [-TermStoreName <String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Description|String|False|Description to use for the term group.
GroupId|Guid|False|GUID to use for the term group; if not specified, or the empty GUID, a random GUID is generated and used.
GroupName|String|True|Name of the taxonomy term group to create.
TermStoreName|String|False|Term store to check; if not specified the default term store is used.
