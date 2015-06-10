#Find-SPOFile
*Topic automatically generated on: 2015-06-04*

Finds a file in the virtual file system of the web.
##Syntax
```powershell
Find-SPOFile -Match <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Match|String|True|Wildcard query|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Find-SPOFile -Match *.master
Will return all masterpages located in the current web.
<!-- Ref: 0716647674C24C4EAA5B638897C18659 -->