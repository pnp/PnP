using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOFileCheckedIn")]
    public class SetFileCheckedIn : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Url = string.Empty;

        [Parameter(Mandatory = false)]
        public CheckinType CheckinType = CheckinType.MajorCheckIn;

        [Parameter(Mandatory = false)]
        public string Comment = "";

        protected override void ExecuteCmdlet()
        {
            PowerShell.Core.SPOWeb.CheckInFile(Url, CheckinType, Comment, this.SelectedWeb, ClientContext);
        }
    }
}
