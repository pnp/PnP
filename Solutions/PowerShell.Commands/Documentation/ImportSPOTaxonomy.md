#Import&#8209;SPOTaxonomy
*Topic automatically generated on: 2015-03-12*

Imports a taxonomy from either a string array or a file
##Syntax
```powershell
Import&#8209;SPOTaxonomy [-Terms [<String[]>]] [-Lcid [<Int32>]] [-TermStoreName [<String>]] [-Delimiter [<String>]] [-SynchronizeDeletions [<SwitchParameter>]]
```
&nbsp;

```powershell
Import&#8209;SPOTaxonomy -Path [<String>] [-Lcid [<Int32>]] [-TermStoreName [<String>]] [-Delimiter [<String>]] [-SynchronizeDeletions [<SwitchParameter>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Delimiter|String|False|
Lcid|Int32|False|
Path|String|True|Specifies a file containing terms per line, in the format as required by the Terms parameter.
SynchronizeDeletions|SwitchParameter|False|If specified, terms that exist in the termset, but are not in the imported data will be removed.
Terms|String[]|False|An array of strings describing termgroup, termset, term, subterms using a default delimiter of '|'.
TermStoreName|String|False|
##Examples

###Example 1
    
PS:> Import-SPOTaxonomy -Terms 'Company|Locations|Stockholm|Central','Company|Locations|Stockholm|North'
Creates a new termgroup, 'Company', a termset 'Locations', a term 'Stockholm' and two subterms: 'Central', and 'North'

###Example 2
    
PS:> Import-SPOTaxonomy -Terms 'Company|Locations|Stockholm'
Creates a new termgroup, 'Company', a termset 'Locations' and a term 'Stockholm'
