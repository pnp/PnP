using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Execute", "SPOQuery")]
    [CmdletHelp("Executes any queued actions / changes on the SharePoint Client Side Object Model Context")]
    public class ExecuteSPOQuery : SPOCmdlet
    {
        protected override void ProcessRecord()
        {
            ClientContext.ExecuteQueryRetry();
        }
    }
}
