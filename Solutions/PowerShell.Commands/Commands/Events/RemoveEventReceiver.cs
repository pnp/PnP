using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOEventReceiver")]
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
                var list = this.SelectedWeb.GetList(List);

                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = list.GetEventReceiverById(Identity.Id);
                    if(eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQuery();
                    }
                }
            }
            else
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    var eventReceiver = this.SelectedWeb.GetEventReceiverById(Identity.Id);
                    if (eventReceiver != null)
                    {
                        eventReceiver.DeleteObject();
                        ClientContext.ExecuteQuery();
                    }
                }
            }
        }

    }

}


