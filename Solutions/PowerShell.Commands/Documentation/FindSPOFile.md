#Find-SPOFile
*Topic last generated: 2015-02-08*

Finds a file in the virtual file system of the web.
##Syntax
    Find-SPOFile -Match [<String>] [-Web [<WebPipeBind>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Match|String|True|Wildcard query
Web|WebPipeBind|False|
##Examples

###Example 1
    
PS:> Find-SPOFile -Match *.master

Will return all masterpages located in the current web.
