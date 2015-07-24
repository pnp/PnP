#Connect-SPOnline
*Topic automatically generated on: 2015-07-21*

Connects to a SharePoint site and creates an in-memory context
##Syntax
```powershell
Connect-SPOnline -ClientId <String> -Tenant <String> -CertificatePath <String> -CertificatePassword <SecureString> [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


```powershell
Connect-SPOnline [-Credentials <CredentialPipeBind>] [-CurrentCredentials [<SwitchParameter>]] [-UseAdfs [<SwitchParameter>]] [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


```powershell
Connect-SPOnline -ClientId <String> -RedirectUri <String> [-ClearTokenCache [<SwitchParameter>]] [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


```powershell
Connect-SPOnline [-Realm <String>] -AppId <String> -AppSecret <String> [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


##Detailed Description
If no credentials have been specified, and the CurrentCredentials parameter has not been specified, you will be prompted for credentials.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AppId|String|True|The Application Client ID to use.|
|AppSecret|String|True|The Application Client Secret to use.|
|CertificatePassword|SecureString|True|Password to the certificate (*.pfx)|
|CertificatePath|String|True|Path to the certificate (*.pfx)|
|ClearTokenCache|SwitchParameter|False|Clears the token cache.|
|ClientId|String|True|The Client ID of the Azure AD Application|
|Credentials|CredentialPipeBind|False|Credentials of the user to connect with. Either specify a PSCredential object or a string. In case of a string value a lookup will be done to the Windows Credential Manager for the correct credentials.|
|CurrentCredentials|SwitchParameter|False|If you want to connect with the current user credentials|
|MinimalHealthScore|Int32|False|Specifies a minimal server healthscore before any requests are executed.|
|Realm|String|False|Authentication realm. If not specified will be resolved from the url specified.|
|RedirectUri|String|True|The Redirect URI of the Azure AD Application|
|RequestTimeout|Int32|False|The request timeout. Default is 180000|
|RetryCount|Int32|False|Defines how often a retry should be executed if the server healthscore is not sufficient.|
|RetryWait|Int32|False|Defines how many seconds to wait before each retry. Default is 5 seconds.|
|SkipTenantAdminCheck|SwitchParameter|False||
|Tenant|String|True|The Azure AD Tenant name,e.g. mycompany.onmicrosoft.com|
|Url|String|True|The Url of the site collection to connect to.|
|UseAdfs|SwitchParameter|False|If you want to connect to your on-premises SharePoint farm using ADFS|
##Examples

###Example 1
    PS:> Connect-SPOnline -Url https://yourtenant.sharepoint.com -Credentials (Get-Credential)
This will prompt for username and password and creates a context for the other PowerShell commands to use.
 

###Example 2
    PS:> Connect-SPOnline -Url http://yourlocalserver -CurrentCredentials
This will use the current user credentials and connects to the server specified by the Url parameter.
    

###Example 3
    PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials 'O365Creds'
This will use credentials from the Windows Credential Manager, as defined by the label 'O365Creds'.
    

###Example 4
    PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials (Get-Credential) -UseAdfs
This will prompt for username and password and creates a context using ADFS to authenticate.
    
<!-- Ref: 3B22375ECB3827FD78FDD02BD95C1871 -->