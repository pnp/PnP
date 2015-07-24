#Set-SPOIndexedProperties
*Topic automatically generated on: 2015-06-11*

Marks values of the propertybag to be indexed by search. Notice that this will overwrite the existing flags, e.g. only the properties you define with the cmdlet will be indexed.
##Syntax
```powershell
Set-SPOIndexedProperties -Keys <List`1> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Keys|List`1|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: EB602FEF58E2826DC391CC5FC26A6697 -->