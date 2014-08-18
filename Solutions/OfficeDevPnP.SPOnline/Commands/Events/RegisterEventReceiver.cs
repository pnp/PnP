using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;
using OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsLifecycle.Register, "SPOEventReceiver")]
    public class RegisterEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "List")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = true)]
        public string Name;

        [Parameter(Mandatory = true)]
        public string Url;

        [Parameter(Mandatory = true)]
        [Alias("Type")]
        public EventReceiverType EventReceiverType;

        [Parameter(Mandatory = true)]
        [Alias("Sync")]
        public EventReceiverSynchronization Synchronization;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                if (List.List != null)
                {
                    WriteObject(SPOEvents.RegisterEventReceiver(List.List, Name, Url, EventReceiverType, Synchronization, Force, ClientContext));
                }
                else if (List.Id != Guid.Empty)
                {
                    var list = SPO.SPOList.GetListById(List.Id, SelectedWeb, ClientContext);
                    WriteObject(SPOEvents.RegisterEventReceiver(list, Name, Url, EventReceiverType, Synchronization, Force, ClientContext));
                }
                else if (!string.IsNullOrEmpty(List.Title))
                {
                    var list = SPO.SPOList.GetListByTitle(List.Title, SelectedWeb, ClientContext);
                    WriteObject(SPOEvents.RegisterEventReceiver(list, Name, Url, EventReceiverType, Synchronization, Force, ClientContext));
                }
                else
                {
                    throw new Exception(Properties.Resources.ListNotFound);
                }
            }
            else
            {
                Microsoft.SharePoint.Client.Web web = SelectedWeb;

                WriteObject(SPOEvents.RegisterEventReceiver(web, Name, Url, EventReceiverType, Synchronization, Force, ClientContext));

            }

        }

    }

}


