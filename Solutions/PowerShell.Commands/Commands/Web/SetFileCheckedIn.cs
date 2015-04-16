using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOFileCheckedIn")]
    [CmdletHelp("Checks in a file", Category = "Webs")]
    public class SetFileCheckedIn : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position=0, ValueFromPipeline=true)]
        public string Url = string.Empty;

        [Parameter(Mandatory = false)]
        public CheckinType CheckinType = CheckinType.MajorCheckIn;

        [Parameter(Mandatory = false)]
        public string Comment = "";

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.CheckInFile(Url, CheckinType, Comment);
        }
    }
}
