using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands.Base
{
    [Cmdlet(VerbsCommon.Get, "SPOContext")]
    [CmdletHelp("Returns a Client Side Object Model context")]
    public class GetSPOContext : PSCmdlet
    {
        protected override void ProcessRecord()
        {
            WriteObject(SPOnlineConnection.CurrentConnection.Context as ClientContext);
        }
    }
}
