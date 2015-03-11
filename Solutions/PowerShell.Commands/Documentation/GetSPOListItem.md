#Get-SPOListItem
*Topic automatically generated on: 2015-03-11*

Retrieves list items
##Syntax
    Get-SPOListItem [-Id [<Int32>]] [-UniqueId [<GuidPipeBind>]] [-Query [<String>]] [-Fields [<String[]>]] [-Web [<WebPipeBind>]] -List [<ListPipeBind>]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Fields|String[]|False|The fields to retrieve. If not specified all fields will be loaded.
Id|Int32|False|The ID of the item to retrieve
List|ListPipeBind|True|The list to query
Query|String|False|The CAML query to execute against the list
UniqueId|GuidPipeBind|False|The unique id (GUID) of the item to retrieve
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOListItem -List Tasks
Retrieves all list items from the tasks lists

###Example 2
    PS:> Get-SPOListItem -List Tasks -Id 1
Retrieves the list item with ID 1 from from the tasks lists. This parameter is ignored if the Query parameter is specified.

###Example 3
    PS:> Get-SPOListItem -List Tasks -UniqueId bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3
Retrieves the list item with unique id bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3 from from the tasks lists. This parameter is ignored if the Query parameter is specified.

###Example 4
    PS:> Get-SPOListItem -List Tasks -Fields "Title","GUID"
Retrieves all list items, but only includes the Title and GUID fields. This parameter is ignored if the Query parameter is specified.

###Example 5
    PS:> Get-SPOListItem -List Tasks -Query "<View><Query><Where><Eq><FieldRef Name='GUID'/><Value Type='Guid'>bd6c5b3b-d960-4ee7-a02c-85dc6cd78cc3</Value></Eq></Where></Query></View>"
Retrieves all list items based on the CAML query specified.
