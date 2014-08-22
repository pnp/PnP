using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Connect", "SPOnline", SupportsShouldProcess = false)]
    [CmdletHelp("Connects to a SharePoint site and creates an in-memory context",
       DetailedDescription = "If no credentials have been specified, and the CurrentCredentials parameter has not been specified, you will be prompted for credentials.")]
    [CmdletExample(
        Code = @"PS:> Connect-SPOnline -Url https://yourtenant.sharepoint.com -Credentials (Get-Credential)",
        Remarks = @"This will prompt for username and password and creates a context for the other PowerShell commands to use.
 ")]
    [CmdletExample(
        Code = @"PS:> Connect-SPOnline -Url http://yourlocalserver -Local",
        Remarks = @"This will use the current user credentials and connects to the server specified by the Url parameter.
    ")]
    [CmdletExample(
       Code = @"PS:> Connect-SPOnline -Url http://yourlocalserver -Credentials 'O365Creds'",
       Remarks = @"This will use credentials from the Windows Credential Manager, as defined by the label 'O365Creds'.
    ")]
    public class ConnectSPOnline : PSCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ParameterSetName = ParameterAttribute.AllParameterSets, ValueFromPipeline = true, HelpMessage = "The Url of the site collection to connect to.")]
        public string Url;

        [Parameter(Mandatory = false, ParameterSetName = "Main", HelpMessage = "Credentials of the user to connect with. Either specify a PSCredential object or a string. In case of a string value a lookup will be done to the Windows Credential Manager for the correct credentials.")]
        public CredentialPipeBind Credentials;

        [Parameter(Mandatory = false, ParameterSetName = "Main", HelpMessage = "If you want to connect with the local / current user credentials")]
        public SwitchParameter CurrentCredentials;

        [Parameter(Mandatory = false, ParameterSetName = "Main", HelpMessage = "Specify if you're connecting to an on-premises environment")]
        public SwitchParameter OnPrem;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Specifies a minimal server healthscore before any requests are executed.")]
        public int MinimalHealthScore = -1;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Defines how often a retry should be executed if the server healthscore is not sufficient.")]
        public int RetryCount = -1;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "Defines how many seconds to wait before each retry. Default is 5 seconds.")]
        public int RetryWait = 5;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "The request timeout. Default is 180000")]
        public int RequestTimeout = 1800000;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public SwitchParameter SkipTenantAdminCheck;

        protected override void ProcessRecord()
        {
            PSCredential creds = null;
            if (Credentials != null)
            {
                creds = Credentials.Credential;

            }
            if (!CurrentCredentials && creds == null)
            {
                creds = this.Host.UI.PromptForCredential(Properties.Resources.EnterYourCredentials, "", "", "");
            }
            SPOnlineConnection.CurrentConnection = SPOnlineConnectionHelper.InstantiateSPOnlineConnection(new Uri(Url), creds, this.Host, CurrentCredentials, OnPrem, MinimalHealthScore, RetryCount, RetryWait, RequestTimeout, SkipTenantAdminCheck);

        }
    }
}
