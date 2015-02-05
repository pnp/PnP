using Microsoft.SharePoint.Client;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsLifecycle.Uninstall, "SPOAppInstance")]
    [CmdletHelp("Removes an app from a site")]
    [CmdletExample(
        Code = @"PS:> Uninstall-SPOAppInstance -Identity $appinstance")]
    [CmdletExample(
        Code = @"PS:> Uninstall-SPOAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe")]
    public class UninstallAppInstance : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "Appinstance or Id of the app to remove.")]
        public AppPipeBind Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            AppInstance instance = null;

            if (Identity.Instance != null)
            {
                instance = Identity.Instance;
            }
            else
            {
                instance = SelectedWeb.GetAppInstanceById(Identity.Id);
            }

            if(instance != null)
            {
                if(!instance.IsObjectPropertyInstantiated("Title"))
                {
                    ClientContext.Load(instance, i => i.Title);
                    ClientContext.ExecuteQueryRetry();
                }
                if (Force || ShouldContinue(string.Format(Properties.Resources.UninstallApp0, instance.Title), Properties.Resources.Confirm))
                {
                    instance.Uninstall();
                    ClientContext.ExecuteQueryRetry();
                }
            }

        }


    }
}
