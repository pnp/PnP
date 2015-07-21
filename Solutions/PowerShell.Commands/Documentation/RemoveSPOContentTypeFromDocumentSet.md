#Remove-SPOContentTypeFromDocumentSet
*Topic automatically generated on: 2015-07-21*

Removes a content type from a document set
##Syntax
```powershell
Remove-SPOContentTypeFromDocumentSet -ContentType <ContentTypePipeBind> -DocumentSet <DocumentSetPipeBind> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind|True|The content type to remove. Either specify name, an id, or a content type object.|
|DocumentSet|DocumentSetPipeBind|True|The document set to remove the content type from. Either specify a name, a document set template object, an id, or a content type object|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Remove-SPOContentTypeFromDocumentSet -ContentType "Test CT" -DocumentSet "Test Document Set"
This will remove the content type called 'Test CT' from the document set called ''Test Document Set'

###Example 2
    PS:> $docset = Remove-SPODocumentSetTemplate -Identity "Test Document Set"
PS:> $ct = Get-SPOContentType -Identity "Test CT"
PS:> Add-SPOContentTypeToDocumentSet -ContentType $ct -DocumentSet $docset
This will remove the content type called 'Test CT' from the document set called ''Test Document Set'

###Example 3
    PS:> Remove-SPOContentTypeToDocumentSet -ContentType 0x0101001F1CEFF1D4126E4CAD10F00B6137E969 -DocumentSet 0x0120D520005DB65D094035A241BAC9AF083F825F3B
This will remove the content type called 'Test CT' from the document set called ''Test Document Set'
<!-- Ref: AC40C3558B15E6E81F61D6D4172F9AA6 -->