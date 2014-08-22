using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOHomePage")]
    public class GetHomePage : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(PowerShell.Core.SPOWeb.GetHomePage(this.SelectedWeb, ClientContext));
        }
    }
}
