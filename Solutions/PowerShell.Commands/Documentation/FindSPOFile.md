#Find-SPOFile
*Topic automatically generated on: 2015-04-29*

Finds a file in the virtual file system of the web.
##Syntax
```powershell
Find-SPOFile -Match <String> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Match|String|True|Wildcard query
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    
PS:> Find-SPOFile -Match *.master

Will return all masterpages located in the current web.
<!-- Ref: 7052E9C35E91464237701DB206B45C47 -->