using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOCustomAction")]
    [CmdletHelp("Returns all or a specific custom action(s)")]
    public class GetCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var actions = SelectedWeb.GetCustomActions();

            if (Identity != null)
            {
                var foundAction = actions.FirstOrDefault(x => x.Id == Identity.Id);
                WriteObject(foundAction);
            }
            else
            {
                WriteObject(actions);
            }
        }
    }
}
