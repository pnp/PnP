#Set-SPOIndexedProperties
*Topic automatically generated on: 2015-04-29*

Marks values of the propertybag to be indexed by search. Notice that this will overwrite the existing flags, e.g. only the properties you define with the cmdlet will be indexed.
##Syntax
```powershell
Set-SPOIndexedProperties -Keys <List`1> [-Web <WebPipeBind>]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Keys|List`1|True|
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
<!-- Ref: F15BE11B0EFF751860BE9BD1503C0DEB -->