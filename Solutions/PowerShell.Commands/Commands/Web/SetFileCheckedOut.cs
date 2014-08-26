using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet("Set", "SPOFileCheckedOut")]
    public class SetFileCheckedOut : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Url = string.Empty;

        protected override void ExecuteCmdlet()
        {
            PowerShell.Core.SPOWeb.CheckOutFile(Url, this.SelectedWeb, ClientContext);

        }
    }
}
