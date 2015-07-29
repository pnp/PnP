#Add-SPOListItem
*Topic automatically generated on: 2015-07-28*

Adds an item to a list
##Syntax
```powershell
Add-SPOListItem [-ContentType <ContentTypePipeBind>] [-Values <Hashtable>] [-Web <WebPipeBind>] -List <ListPipeBind>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|False|Specify either the name, ID or an actual content type.|
|List|ListPipeBind|True|The ID, Title or Url of the list.|
|Values|Hashtable|False||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    Add-SPOListItem -List "Demo List" -Values @{"Title" = "Test Title"; "Category"="Test Category"}
Adds a new list item to the "Demo List", and sets both the Title and Category fields with the specified values.

###Example 2
    Add-SPOListItem -List "Demo List" -ContentType "Company" -Values @{"Title" = "Test Title"; "Category"="Test Category"}
Adds a new list item to the "Demo List", sets the content type to "Company" and sets both the Title and Category fields with the specified values.
<!-- Ref: 5385432995C5D45D413370889A82ED98 -->