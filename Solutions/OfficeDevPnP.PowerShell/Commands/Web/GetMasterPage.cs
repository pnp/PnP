using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOMasterPage")]
    public class GetMasterPage : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            WriteObject(PowerShell.Core.SPOWeb.GetMasterPage(this.SelectedWeb, ClientContext));
        }
    }
}
