using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOEventReceiver")]
    public class GetEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "List")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                var list = this.SelectedWeb.GetList(List);

                if (list != null)
                {
                    if (Identity == null)
                    {
                        var query = ClientContext.LoadQuery(list.EventReceivers);
                        ClientContext.ExecuteQuery();
                        WriteObject(query, true);
                    }
                    else
                    {
                        WriteObject(list.GetEventReceiverById(Identity.Id));
                    }
                }
            }
            else
            {
                if (Identity == null)
                {
                    var query = ClientContext.LoadQuery(this.SelectedWeb.EventReceivers);
                    ClientContext.ExecuteQuery();
                    WriteObject(query, true);
                }
                else
                {
                    WriteObject(this.SelectedWeb.GetEventReceiverById(Identity.Id));
                }
            }

        }
    }
}


