using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Get, "SPOContext")]
    [CmdletHelp("Returns a Client Side Object Model context", Category = "Base Cmdlets")]
    public class GetSPOContext : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(SPOnlineConnection.CurrentConnection.Context);
        }
    }
}
