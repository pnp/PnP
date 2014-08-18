using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
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


namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOList")]
    public class RemoveList : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID or Title of the list.")]
        public SPOListPipeBind Identity = new SPOListPipeBind();

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;
        protected override void ExecuteCmdlet()
        {
            if (Identity != null)
            {
                List list = null;
                if (Identity.Id != Guid.Empty)
                {
                    list = SPO.SPOList.GetListById(Identity.Id, SelectedWeb, ClientContext);
                }
                else if (Identity.List != null)
                {
                    list = Identity.List;
                }
                else if (!string.IsNullOrEmpty(Identity.Title))
                {
                    list = SPO.SPOList.GetListByTitle(Identity.Title, SelectedWeb, ClientContext);

                }
                if (list != null)
                {
                    if (Force || ShouldContinue(Properties.Resources.RemoveList, Properties.Resources.Confirm))
                    {
                        list.DeleteObject();
                        ClientContext.ExecuteQuery();
                    }
                }
            }
        }
    }

}
