#Get-SPODocumentSetTemplate
*Topic automatically generated on: 2015-08-04*

Retrieves a document set template
##Syntax
```powershell
Get-SPODocumentSetTemplate [-Web [<WebPipeBind>]] -Identity [<DocumentSetPipeBind>]
```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Identity|DocumentSetPipeBind|True|Either specify a name, an id, a document set template object or a content type object
Web|WebPipeBind|False|The web to apply the command to. Omit this parameter to use the current web.
##Examples

###Example 1
    PS:> Get-SPODocumentSetTemplate -Identity "Test Document Set"


###Example 2
    PS:> Get-SPODocumentSetTemplate -Identity "0x0120D520005DB65D094035A241BAC9AF083F825F3B"

