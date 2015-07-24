#Get-SPOAzureADManifestKeyCredentials
*Topic automatically generated on: 2015-07-16*

Creates the JSON snippet that is required for the manifest json file for Azure WebApplication / WebAPI apps
##Syntax
```powershell
Get-SPOAzureADManifestKeyCredentials -CertPath <String>
```


##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|CertPath|String|True||
##Examples

###Example 1
    PS:> Get-SPOAzureADManifestKeyCredentials -CertPath .\mycert.cer
Output the JSON snippet which needs to be replaced in the application manifest file

###Example 2
    PS:> Get-SPOAzureADManifestKeyCredentials -CertPath .\mycert.cer | Set-Clipboard
Output the JSON snippet which needs to be replaced in the application manifest file and copies it to the clipboard
<!-- Ref: C18CE2F7CF406256EF8C45CC306EA67A -->