using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Request, "SPOReIndexWeb")]
    [CmdletHelp("Marks the web for full indexing during the next incremental crawl", Category = "Webs")]
    public class RequestReIndexWeb : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.ReIndexWeb();
        }
    }
}
