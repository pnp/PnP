using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Execute", "SPOQuery")]
    [CmdletHelp("Executes any queued actions / changes on the SharePoint Client Side Object Model Context", Category = "Base Cmdlets")]
    public class ExecuteSPOQuery : SPOCmdlet
    {
        protected override void ProcessRecord()
        {
            ClientContext.ExecuteQueryRetry();
        }
    }
}
