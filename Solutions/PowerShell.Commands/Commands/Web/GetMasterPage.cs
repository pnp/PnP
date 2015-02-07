using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOMasterPage")]
    public class GetMasterPage : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            ClientContext.Load(SelectedWeb, w => w.MasterUrl, w => w.CustomMasterUrl);
            ClientContext.ExecuteQueryRetry();

            WriteObject(new {SelectedWeb.MasterUrl, SelectedWeb.CustomMasterUrl });
        }
    }
}
