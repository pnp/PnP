using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOApp")]
    [CmdletHelp("Removes an app from a site")]
    [CmdletExample(
        Code = @"PS:> Remove-SPOnlineApp -Identity $appinstance")]
    [CmdletExample(
        Code = @"PS:> Remove-SPOnlineApp -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe")]
    public class RemoveApp : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Appinstance or Id of the app to remove.")]
        public AppPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (Identity.Instance != null)
            {
                Identity.Instance.Uninstall();
                ClientContext.ExecuteQuery();
            }
            else
            {
                var instance = this.SelectedWeb.GetAppInstanceById(Identity.Id);
                instance.Uninstall();
                ClientContext.ExecuteQuery();
            }
        }


    }
}
