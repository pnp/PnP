using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOCustomAction")]
    [CmdletHelp("Returns all or a specific custom action(s)")]
    public class GetCustomAction : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = false)]
        public CustomActionScope Scope = CustomActionScope.Web;

        protected override void ExecuteCmdlet()
        {
            List<UserCustomAction> actions = null;

            if (Scope == CustomActionScope.Web)
            {
                actions = SelectedWeb.GetCustomActions().ToList();
            }
            else
            {
                actions = ClientContext.Site.GetCustomActions().ToList();
            }

            if (Identity != null)
            {
                var foundAction = actions.FirstOrDefault(x => x.Id == Identity.Id);
                WriteObject(foundAction, true);
            }
            else
            {
                WriteObject(actions,true);
            }
        }
    }
}
