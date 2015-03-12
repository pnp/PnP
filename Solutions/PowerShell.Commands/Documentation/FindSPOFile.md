#Find&#8209;SPOFile
*Topic automatically generated on: 2015-03-12*

Finds a file in the virtual file system of the web.
##Syntax
```powershell
Find&#8209;SPOFile -Match [<String>] [-Web [<WebPipeBind>]]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Match|String|True|Wildcard query
Web|WebPipeBind|False|The web to apply the command to. Leave empty to use the current web.
##Examples

###Example 1
    
PS:> Find-SPOFile -Match *.master

Will return all masterpages located in the current web.
