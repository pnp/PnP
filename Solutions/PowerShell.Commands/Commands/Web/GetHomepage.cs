using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOHomePage")]
    [CmdletHelp("Returns the URL to the home page", Category = "Webs")]
    public class GetHomePage : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var folder = SelectedWeb.RootFolder;

            ClientContext.Load(folder, f => f.WelcomePage);

            ClientContext.ExecuteQueryRetry();

            if (string.IsNullOrEmpty(folder.WelcomePage))
            {
                WriteObject("default.aspx");
            }
            else
            {
                WriteObject(folder.WelcomePage);
            }
        }
    }
}
