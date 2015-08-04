#Get-SPOStoredCredential
*Topic automatically generated on: 2015-06-11*

Returns a stored credential from the Windows Credential Manager
##Syntax
```powershell
Get-SPOStoredCredential -Name <String> [-Type <CredentialType>]
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|Name|String|True|The credential to retrieve.|
|Type|CredentialType|False|The object type of the credential to return from the Credential Manager. Possible valus are 'O365', 'OnPrem' or 'PSCredential'|
##Examples

###Example 1
    PS:> Get-SPOnlineStoredCredential -Name O365
Returns the credential associated with the specified identifier
<!-- Ref: 215D94FE875F661197E31D96027B8A40 -->