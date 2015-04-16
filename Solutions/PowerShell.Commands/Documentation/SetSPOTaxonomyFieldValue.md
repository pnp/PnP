#Set-SPOTaxonomyFieldValue
*Topic automatically generated on: 2015-04-02*

Sets a taxonomy term value in a listitem field
##Syntax
```powershell
Set-SPOTaxonomyFieldValue -Label [<String>] -TermId [<GuidPipeBind>] -ListItem [<ListItem>] -InternalFieldName [<String>]
```
&nbsp;

```powershell
Set-SPOTaxonomyFieldValue -TermPath [<String>] -ListItem [<ListItem>] -InternalFieldName [<String>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
InternalFieldName|String|True|The internal name of the field
Label|String|True|The Label value of the term
ListItem|ListItem|True|The list item to set the field value to
TermId|GuidPipeBind|True|The Id of the Term
TermPath|String|True|A path in the form of GROUPLABEL|TERMSETLABEL|TERMLABEL
##Examples

###Example 1
    
PS:> Set-SPOTaxonomyFieldValue -ListItem $item -InternalFieldName 'Department' -Label 'HR'
    


###Example 2
    
PS:> Set-SPOTaxonomyFieldValue -ListItem $item -InternalFieldName 'Department' -TermPath 'CORPORATE|DEPARTMENTS|HR'
    

