#Export-SPOTermGroupToXml
*Topic automatically generated on: 2015-06-11*

Exports a taxonomy TermGroup to either the output or to an XML file.
##Syntax
```powershell
Export-SPOTermGroupToXml [-Identity <TermGroupPipeBind>] [-Out <String>] [-FullTemplate [<SwitchParameter>]] [-Encoding <Encoding>] [-Force [<SwitchParameter>]]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Encoding|Encoding|False||
|Force|SwitchParameter|False|Overwrites the output file if it exists.|
|FullTemplate|SwitchParameter|False|If specified, a full provisioning template structure will be returned|
|Identity|TermGroupPipeBind|False|The ID or name of the termgroup|
|Out|String|False|File to export the data to.|
##Examples

###Example 1
    PS:> Export-SPOTermGroupToXml
Exports all term groups in the default site collection term store to the standard output

###Example 2
    PS:> Export-SPOTermGroupToXml -Out output.xml
Exports all term groups in the default site collection term store to the file 'output.xml' in the current folder

###Example 3
    PS:> Export-SPOTermGroupToXml -Out c:\output.xml -TermGroup "Test Group"
Exports the term group with the specified name to the file 'output.xml' located in the root folder of the C: drive.
<!-- Ref: 62E43DCD132E8577F506B58A67A37AE2 -->