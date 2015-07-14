#Connect-SPOnline
*Topic automatically generated on: 2015-07-14*

Connects to a SharePoint site and creates an in-memory context
##Syntax
```powershell
Connect-SPOnline -RelyingPartyIdentifier <String> -AdfsHostName <String> [-Credentials <CredentialPipeBind>] [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


```powershell
Connect-SPOnline [-CurrentCredentials [<SwitchParameter>]] [-Credentials <CredentialPipeBind>] [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


```powershell
Connect-SPOnline [-Realm <String>] -AppId <String> -AppSecret <String> [-Credentials <CredentialPipeBind>] [-MinimalHealthScore <Int32>] [-RetryCount <Int32>] [-RetryWait <Int32>] [-RequestTimeout <Int32>] [-SkipTenantAdminCheck [<SwitchParameter>]] -Url <String>
```


##Detailed Description
If no credentials have been specified, and the CurrentCredentials parameter has not been specified, you will be prompted for credentials.

##Parameters
Parameter|Type|Required|Description
---------|----|--------|-----------
|AdfsHostName|String|True|DNS name of the ADFS server which the SharePoint farm uses for authentication.|
|AppId|String|True|The Application Client ID to use.|
|AppSecret|String|True|The Application Client Secret to use.|
|Credentials|CredentialPipeBind|False|Credentials of the user to connect with. Either specify a PSCredential object or a string. In case of a string value a lookup will be done to the Windows Credential Manager for the correct credentials.|
|CurrentCredentials|SwitchParameter|False|If you want to connect with the current user credentials|
|MinimalHealthScore|Int32|False|Specifies a minimal server healthscore before any requests are executed.|
|Realm|String|False|Authentication realm. If not specified will be resolved from the url specified.|
|RelyingPartyIdentifier|String|True|Relying party identifier of the SharePoint farm inside ADFS.|
|RequestTimeout|Int32|False|The request timeout. Default is 180000|
|RetryCount|Int32|False|Defines how often a retry should be executed if the server healthscore is not sufficient.|
|RetryWait|Int32|False|Defines how many seconds to wait before each retry. Default is 5 seconds.|
|SkipTenantAdminCheck|SwitchParameter|False||
|Url|String|True|The Url of the site collection to connect to.|
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
    PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials (Get-Credential) -AdfsHostName 'sts.consoso.com' -RelyingPartyIdentifier 'urn:sharepoint:contoso'
This will prompt for username and password and creates a context using ADFS to authenticate.
    
<!-- Ref: EF4E11F0267F78DF82614FC130D94195 -->