using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.IO;
using System.Management.Automation;
using System.Net;
using System.Security;
using Microsoft.SharePoint.Client.CompliancePolicy;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Connect", "SPOnline", SupportsShouldProcess = false)]
    [CmdletHelp("Connects to a SharePoint site and creates an in-memory context",
       DetailedDescription = "If no credentials have been specified, and the CurrentCredentials parameter has not been specified, you will be prompted for credentials.", Category = "Base Cmdlets")]
    [CmdletExample(
        Code = @"PS:> Connect-SPOnline -Url https://yourtenant.sharepoint.com -Credentials (Get-Credential)",
        Remarks = @"This will prompt for username and password and creates a context for the other PowerShell commands to use.
 ", SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Connect-SPOnline -Url http://yourlocalserver -CurrentCredentials",
        Remarks = @"This will use the current user credentials and connects to the server specified by the Url parameter.
    ", SortOrder = 2)]
    [CmdletExample(
       Code = @"PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials 'O365Creds'",
       Remarks = @"This will use credentials from the Windows Credential Manager, as defined by the label 'O365Creds'.
    ", SortOrder = 3)]
    [CmdletExample(
     Code = @"PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials (Get-Credential) -AdfsHostName 'sts.consoso.com' -RelyingPartyIdentifier 'urn:sharepoint:contoso'",
     Remarks = @"This will prompt for username and password and creates a context using ADFS to authenticate.
    ", SortOrder = 4)]
    public class ConnectSPOnline : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "The Url of the site collection to connect to.")]
        public string Url;

        [Parameter(Mandatory = false, ParameterSetName = "Main", HelpMessage = "Credentials of the user to connect with. Either specify a PSCredential object or a string. In case of a string value a lookup will be done to the Windows Credential Manager for the correct credentials.")]
        public CredentialPipeBind Credentials;

        [Parameter(Mandatory = false, ParameterSetName = "Main", HelpMessage = "If you want to connect with the current user credentials")]
        public SwitchParameter CurrentCredentials;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Specifies a minimal server healthscore before any requests are executed.")]
        public int MinimalHealthScore = -1;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Defines how often a retry should be executed if the server healthscore is not sufficient.")]
        public int RetryCount = -1;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Defines how many seconds to wait before each retry. Default is 5 seconds.")]
        public int RetryWait = 5;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "The request timeout. Default is 180000")]
        public int RequestTimeout = 1800000;

        [Parameter(Mandatory = false, ParameterSetName = "Token", HelpMessage = "Authentication realm. If not specified will be resolved from the url specified.")]
        public string Realm;

        [Parameter(Mandatory = true, ParameterSetName = "Token", HelpMessage = "The Application Client ID to use.")]
        public string AppId;

        [Parameter(Mandatory = true, ParameterSetName = "Token", HelpMessage = "The Application Client Secret to use.")]
        public string AppSecret;

        [Parameter(Mandatory = true, ParameterSetName = "ADFS", HelpMessage = "Relying party identifier of the SharePoint farm inside ADFS.")]
        public string RelyingPartyIdentifier;

        [Parameter(Mandatory = true, ParameterSetName = "ADFS", HelpMessage = "DNS name of the ADFS server which the SharePoint farm uses for authentication.")]
        public string AdfsHostName;

#if !CLIENTSDKV15
        [Parameter(Mandatory = true, ParameterSetName = "NativeAAD", HelpMessage = "The Client ID of the Azure AD Application")]
        [Parameter(Mandatory = true, ParameterSetName = "AppOnlyAAD", HelpMessage = "The Client ID of the Azure AD Application")]
        public string ClientId;

        [Parameter(Mandatory = true, ParameterSetName = "NativeAAD", HelpMessage = "The Redirect URI of the Azure AD Application")]
        public string RedirectUri;

        [Parameter(Mandatory = true, ParameterSetName = "AppOnlyAAD", HelpMessage = "The Azure AD Tenant name,e.g. mycompany.onmicrosoft.com")]
        public string Tenant;

        [Parameter(Mandatory = true, ParameterSetName = "AppOnlyAAD", HelpMessage = "Path to the certificate (*.pfx)")]
        public string CertificatePath;

        [Parameter(Mandatory = true, ParameterSetName = "AppOnlyAAD", HelpMessage = "Password to the certificate (*.pfx)")]
        public SecureString CertificatePassword;

        [Parameter(Mandatory = false, ParameterSetName = "NativeAAD", HelpMessage = "Clears the token cache.")]
        public SwitchParameter ClearTokenCache;
#endif
        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public SwitchParameter SkipTenantAdminCheck;

        protected override void ProcessRecord()
        {
            PSCredential creds = null;
            if (Credentials != null)
            {
                creds = Credentials.Credential;
            }

            if (ParameterSetName == "Token")
            {
                SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InstantiateSPOnlineConnection(new Uri(Url), Realm, AppId, AppSecret, Host, MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);
            }
            else if (ParameterSetName == "ADFS")
            {
                creds = GetCredentials();
                if (creds == null)
                {
                    creds = Host.UI.PromptForCredential(Properties.Resources.EnterYourCredentials, "", "", "");
                }
                SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InstantiateSPOnlineConnection(new Uri(Url), AdfsHostName, RelyingPartyIdentifier, creds, Host, MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);
            }
#if !CLIENTSDKV15
            else if (ParameterSetName == "NativeAAD")
            {
                if (ClearTokenCache)
                {
                    string appDataFolder = Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
                    string configFile = Path.Combine(appDataFolder, "OfficeDevPnP.PowerShell\\tokencache.dat");
                    if (File.Exists(configFile))
                    {
                        File.Delete(configFile);
                    }
                }
                SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InitiateAzureADNativeApplicationConnection(new Uri(Url), ClientId, new Uri(RedirectUri), MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);
            }
            else if (ParameterSetName == "AppOnlyAAD")
            {
                SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InitiateAzureADAppOnlyConnection(new Uri(Url), ClientId, Tenant, CertificatePath, CertificatePassword, MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);
            }
#endif
            else
            {
                if (!CurrentCredentials && creds == null)
                {
                    creds = GetCredentials();
                    if (creds == null)
                    {
                        creds = Host.UI.PromptForCredential(Properties.Resources.EnterYourCredentials, "", "", "");
                    }
                }
                SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InstantiateSPOnlineConnection(new Uri(Url), creds, Host, CurrentCredentials, MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);
            }

        }

        private PSCredential GetCredentials()
        {
            PSCredential creds = null;

            var connectionURI = new Uri(Url);

            // Try to get the credentials by full url

            creds = Utilities.CredentialManager.GetCredential(Url);
            if (creds == null)
            {
                // Try to get the credentials by splitting up the path
                var pathString = string.Format("{0}://{1}", connectionURI.Scheme, connectionURI.IsDefaultPort ? connectionURI.Host : string.Format("{0}:{1}", connectionURI.Host, connectionURI.Port));
                var path = connectionURI.AbsolutePath;
                while (path.IndexOf('/') != -1)
                {
                    path = path.Substring(0, path.LastIndexOf('/'));
                    if (!string.IsNullOrEmpty(path))
                    {
                        var pathUrl = string.Format("{0}{1}", pathString, path);
                        creds = Utilities.CredentialManager.GetCredential(pathUrl);
                        if (creds != null)
                        {
                            break;
                        }
                    }
                }

                if (creds == null)
                {
                    // Try to find the credentials by schema and hostname
                    creds = Utilities.CredentialManager.GetCredential(connectionURI.Scheme + "://" + connectionURI.Host);

                    if (creds == null)
                    {
                        // try to find the credentials by hostname
                        creds = Utilities.CredentialManager.GetCredential(connectionURI.Host);
                    }
                }

            }

            return creds;
        }
    }
}
