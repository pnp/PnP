using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Utilities;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Get", "SPOStoredCredential")]
    [CmdletHelp("Returns a stored credential from the Windows Credential Manager")]
    [CmdletExample(Code = "PS:> Get-SPOnlineStoredCredential -Name O365", Remarks = "Returns the credential associated with the specified identifier")]
    public class GetStoredCredential : PSCmdlet
    {
        [Parameter(Mandatory = false, HelpMessage = "The credential to retrieve.")]
        public string Name { get; set; }

        protected override void ProcessRecord()
        {
            WriteObject(CredentialManager.GetCredential(Name));
        }
    }
}
