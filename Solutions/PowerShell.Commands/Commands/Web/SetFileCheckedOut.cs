using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet("Set", "SPOFileCheckedOut")]
    public class SetFileCheckedOut : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true)]
        public string Url = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.CheckOutFile(Url);
        }
    }
}
