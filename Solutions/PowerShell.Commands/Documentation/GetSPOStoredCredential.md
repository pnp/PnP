#Get-SPOStoredCredential
*Topic last generated: 2015-02-08*

Returns a stored credential from the Windows Credential Manager
##Syntax
    Get-SPOStoredCredential -Name [<String>] [-Type [<CredentialType>]]

&nbsp;

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
Name|String|True|The credential to retrieve.
Type|CredentialType|False|The type of credential to retrieve from the Credential Manager. Possible valus are 'O365', 'OnPrem' or 'PSCredential'
##Examples

###Example 1
    PS:> Get-SPOnlineStoredCredential -Name O365
Returns the credential associated with the specified identifier
