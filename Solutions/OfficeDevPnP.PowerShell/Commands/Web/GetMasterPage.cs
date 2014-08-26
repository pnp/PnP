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
            ClientContext.Load(this.SelectedWeb, w => w.MasterUrl, w => w.CustomMasterUrl);
            ClientContext.ExecuteQuery();

            WriteObject(new { MasterUrl = this.SelectedWeb.MasterUrl, CustomMasterUrl = this.SelectedWeb.CustomMasterUrl });
        }
    }
}
