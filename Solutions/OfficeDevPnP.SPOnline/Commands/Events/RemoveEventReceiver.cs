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

        [Parameter(Mandatory = true)]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            Microsoft.SharePoint.Client.List list = null;
            if (List.List != null)
            {
                list = List.List;
            }
            else if (List.Id != Guid.Empty)
            {
                list = SPO.SPOList.GetListById(List.Id, SelectedWeb, ClientContext);
            }
            else if (!string.IsNullOrEmpty(List.Title))
            {
                list = SPO.SPOList.GetListByTitle(List.Title, SelectedWeb, ClientContext);
            }

            if (list != null)
            {
                if (Force || ShouldContinue(Properties.Resources.RemoveEventReceiver, Properties.Resources.Confirm))
                {
                    SPOEvents.RemoveEventReceiver(list, Identity.Id, ClientContext);
                }
            }
            else
            {
                throw new Exception(Properties.Resources.ListNotFound);
            }
        }

    }

}


