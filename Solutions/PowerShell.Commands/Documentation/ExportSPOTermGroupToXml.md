#Export-SPOTermGroupToXml
*Topic automatically generated on: 2015-05-25*

Exports a taxonomy TermGroup to either the output or to an XML file.
##Syntax
```powershell
Export-SPOTermGroupToXml [-Identity <TermGroupPipeBind>] [-Out <String>] [-FullTemplate [<SwitchParameter>]] [-Encoding <Encoding>] [-Force [<SwitchParameter>]]```
&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Encoding|Encoding|False|
Force|SwitchParameter|False|Overwrites the output file if it exists.
FullTemplate|SwitchParameter|False|If specified, a full provisioning template structure will be returned
Identity|TermGroupPipeBind|False|The ID or name of the termgroup
Out|String|False|File to export the data to.
##Examples

###Example 1
    PS:> Export-SPOTermGroupToXml -Out output.xml
Exports all term groups in the default site collection term store to the file 'output.xml' in the current folder

###Example 2
    PS:> Export-SPOTermGroupToXml -Out c:\output.xml -TermGroup "Test Group"
Exports the term group with the specified name to the file 'output.xml' located in the root folder of the C: drive.

###Example 3
    PS:> Export-SPOTermGroupToXml
Exports all term groups in the default site collection term store to the standard output
<!-- Ref: 0767E388BD38C21622BBB3511B03FCB2 -->