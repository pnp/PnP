using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Request, "SPOReIndexWeb")]
    public class RequestReIndexWeb : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.ReIndexWeb();
        }
    }
}
