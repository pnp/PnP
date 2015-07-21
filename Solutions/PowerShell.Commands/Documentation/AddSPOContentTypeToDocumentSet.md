#Add-SPOContentTypeToDocumentSet
*Topic automatically generated on: 2015-07-21*

Adds a content type to a document set
##Syntax
```powershell
Add-SPOContentTypeToDocumentSet -ContentType <ContentTypePipeBind[]> -DocumentSet <DocumentSetPipeBind> [-Web <WebPipeBind>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|ContentType|ContentTypePipeBind[]|True|The content type to add. Either specify name, an id, or a content type object.|
|DocumentSet|DocumentSetPipeBind|True|The document set to add the content type to. Either specify a name, a document set template object, an id, or a content type object|
|Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.|
##Examples

###Example 1
    PS:> Add-SPOContentTypeToDocumentSet -ContentType "Test CT" -DocumentSet "Test Document Set"
This will add the content type called 'Test CT' to the document set called ''Test Document Set'

###Example 2
    PS:> $docset = Get-SPODocumentSetTemplate -Identity "Test Document Set"
PS:> $ct = Get-SPOContentType -Identity "Test CT"
PS:> Add-SPOContentTypeToDocumentSet -ContentType $ct -DocumentSet $docset
This will add the content type called 'Test CT' to the document set called ''Test Document Set'

###Example 3
    PS:> Add-SPOContentTypeToDocumentSet -ContentType 0x0101001F1CEFF1D4126E4CAD10F00B6137E969 -DocumentSet 0x0120D520005DB65D094035A241BAC9AF083F825F3B
This will add the content type called 'Test CT' to the document set called ''Test Document Set'
<!-- Ref: 172C8549CAB45241BC91DA89F0F48D30 -->