using OfficeDevPnP.SPOnline.Core;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOEventReceiver")]
    public class RemoveEventReceiver : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true, ParameterSetName="List")]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "List")
            {
                var list = this.SelectedWeb.GetList(List);

                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    SPOEvents.RemoveEventReceiver(list, Identity.Id, ClientContext);
                }
            }
            else
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    SPOEvents.RemoveEventReceiver(this.SelectedWeb, Identity.Id, ClientContext);
                }
            }
        }

    }

}


