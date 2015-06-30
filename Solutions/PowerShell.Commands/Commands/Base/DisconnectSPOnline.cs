using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet("Disconnect", "SPOnline")]

    [CmdletHelp("Disconnects the context", Category = "Base Cmdlets")]
    [CmdletExample(
        Code = @"PS:> Disconnect-SPOnline", SortOrder = 1)]
    public class DisconnectSPOnline : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            if (!DisconnectCurrentService())
                throw new InvalidOperationException(Properties.Resources.NoConnectionToDisconnect);
        }

        internal static bool DisconnectCurrentService()
        {
            if (SPOnlineConnection.CurrentConnection == null)
                return false;
            SPOnlineConnection.CurrentConnection = null;
            return true;
        }
    }
}
