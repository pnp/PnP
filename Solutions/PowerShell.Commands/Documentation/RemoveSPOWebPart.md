#Remove-SPOWebPart
*Topic automatically generated on: 2015-06-03*

Removes a webpart from a page
##Syntax
```powershell
Remove-SPOWebPart -Identity <GuidPipeBind> -PageUrl <String> [-Web <WebPipeBind>]
```


```powershell
Remove-SPOWebPart -Name <String> -PageUrl <String> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Identity|GuidPipeBind|True||
|Name|String|True||
|PageUrl|String|True||
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
<!-- Ref: E4CDCEF96479A4DAB33B8E8D6F311B50 -->