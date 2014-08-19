using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOCustomAction")]
    [CmdletHelp("Returns all or a specific custom action(s)")]
    public class GetCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var actions = SPOnline.Core.SPOWeb.GetCustomActions(this.SelectedWeb, ClientContext);

            if (Identity != null)
            {
                var foundAction = actions.Where(x => x.Id == Identity.Id).FirstOrDefault();
                WriteObject(foundAction);
            }
            else
            {
                WriteObject(actions);
            }
        }
    }
}
