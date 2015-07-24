#Remove-SPOIndexedProperty
*Topic automatically generated on: 2015-07-01*

Removes a key from propertybag to be indexed by search. The key and it's value retain in the propertybag, however it will not be indexed anymore.
##Syntax
```powershell
Remove-SPOIndexedProperty [-Web <WebPipeBind>] -Key <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Key|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: 1CA7508466063A048BC0EF35887A92CC -->