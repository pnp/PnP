using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWeb")]
    [CmdletHelp("Removes a subweb in the current web", Category = "Webs")]
    [CmdletExample(
        Code = @"PS:> Remove-SPOWeb -Url projectA",
        Remarks = "Remove a web",
        SortOrder = 1)]

    public class RemoveWeb : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The Url of the web")]
        public string Url;

        protected override void ExecuteCmdlet()
        {
            var web = SelectedWeb.DeleteWeb(Url);
            ClientContext.ExecuteQueryRetry();
        }
    }
}
