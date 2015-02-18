using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOEventReceiver")]
    [CmdletHelp("Removes/unregisters a specific event receiver")]
    [CmdletExample(
      Code = @"PS:> Remove-SPOEventReceiver -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the current web", SortOrder = 1)]
    [CmdletExample(
      Code = @"PS:> Remove-SPOEventReceiver -List ProjectList -Identity fb689d0e-eb99-4f13-beb3-86692fd39f22",
      Remarks = @"This will remove an event receiver with id fb689d0e-eb99-4f13-beb3-86692fd39f22 from the list with name ""ProjectList""", SortOrder = 1)]
    public class RemoveEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false, ParameterSetName="List")]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                var list = SelectedWeb.GetList(List);

                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = list.GetEventReceiverById(Identity.Id);
                    if(eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
            else
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = SelectedWeb.GetEventReceiverById(Identity.Id);
                    if (eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQueryRetry();
                    }
                }
            }
        }

    }

}


