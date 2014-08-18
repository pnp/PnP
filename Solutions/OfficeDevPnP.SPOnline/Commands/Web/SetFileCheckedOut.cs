using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet("Set", "SPOFileCheckedOut")]
    public class SetFileCheckedOut : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Url = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SPOnline.Core.SPOWeb.CheckOutFile(Url, this.SelectedWeb, ClientContext);

        }
    }
}
