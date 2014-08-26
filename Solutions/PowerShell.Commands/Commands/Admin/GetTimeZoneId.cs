using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Core;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTimeZoneId")]
    [CmdletHelp("Adds a SharePoint App to a site",
        Details = "This commands requires that you have an app package to deploy")]
    [CmdletExample(
        Code = @"PS:> Add-SPOnlineApp -Path c:\files\demo.app -LoadOnly",
        Remarks = @"This will load the app in the demo.app package, but will not install it to the site.
 ")]
    [CmdletExample(
        Code = @"PS:> Add-SPOnlineApp -Path c:\files\demo.app -Force",
        Remarks = @"This load first activate the app sideloading feature, upload and install the app, and deactivate the app sideloading feature.
    ")]
    public class GetTimeZoneId : PSCmdlet
    {
        [Parameter(Mandatory = false)]
        public string Match;

        protected override void ProcessRecord()
        {
            if (Match != null)
            {
                WriteObject(SPOAdmin.FindZone(Match));
            }
            else
            {
                WriteObject(SPOAdmin.AllZones());
            }
        }






    }
}
