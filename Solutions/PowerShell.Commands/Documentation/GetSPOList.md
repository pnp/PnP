#Get-SPOList
*Topic automatically generated on: 2015-03-12*

Returns a List object
##Syntax
```powershell
Get-SPOList [-Web [<WebPipeBind>]] [-Identity [<ListPipeBind>]]
```
&nbsp;

##Detailed Description
Returns a list object.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|ListPipeBind|False|The ID or Url of the list.
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    PS:> Get-SPOList
Returns all lists in the current web

###Example 2
    PS:> Get-SPOList -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe
Returns a list with the given id.

###Example 3
    PS:> Get-SPOList -Identity /Lists/Announcements
Returns a list with the given url.
