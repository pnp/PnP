using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System.Linq;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOAppInstance")]
    [CmdletHelp("Returns a SharePoint App Instance")]
    [CmdletExample(
        Code = @"PS:> Get-SPOnlineAppInstance",
        Remarks = @"This will return all app instances in the site.
 ", SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOnlineAppInstance -Identity 99a00f6e-fb81-4dc7-8eac-e09c6f9132fe",
        Remarks = @"This will return an app instance with the specified id.
    ", SortOrder = 2)]
    public class GetAppInstance : SPOWebCmdlet
    {

        [Parameter(Mandatory = false, Position=0, ValueFromPipeline = true, HelpMessage = "The Id of the App Instance")]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            
            if (Identity != null)
            {
                var instance = this.SelectedWeb.GetAppInstanceById(Identity.Id);
                ClientContext.Load(instance);
                ClientContext.ExecuteQuery();
                WriteObject(instance);
            }
            else
            {
                var instances = this.SelectedWeb.GetAppInstances();
                if (instances.Count > 1)
                {
                    WriteObject(instances,true);
                }
                else if (instances.Count == 1)
                {
                    WriteObject(instances[0]);
                }
            }
        }
    }
}
