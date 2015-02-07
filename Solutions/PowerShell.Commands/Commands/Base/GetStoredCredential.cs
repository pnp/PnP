using System.Management.Automation;
using OfficeDevPnP.Core.Utilities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Get", "SPOStoredCredential")]
    [CmdletHelp("Returns a stored credential from the Windows Credential Manager")]
    [CmdletExample(Code = "PS:> Get-SPOnlineStoredCredential -Name O365", Remarks = "Returns the credential associated with the specified identifier")]
    public class GetStoredCredential : PSCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The credential to retrieve.")]
        public string Name;

        [Parameter(Mandatory = false, HelpMessage = "The type of credential to retrieve from the Credential Manager. Possible valus are 'O365', 'OnPrem' or 'PSCredential'")]
        public CredentialType Type = CredentialType.O365;

        protected override void ProcessRecord()
        {
            switch (Type)
            {
                case CredentialType.O365:
                    {
                        WriteObject(CredentialManager.GetSharePointOnlineCredential(Name));
                        break;
                    }
                case CredentialType.OnPrem:
                    {
                        WriteObject(CredentialManager.GetCredential(Name));
                        break;
                    }
                case CredentialType.PSCredential:
                    {
                        WriteObject(Utilities.CredentialManager.GetCredential(Name));
                        break;
                    }
            }
        }
    }
}
