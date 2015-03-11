using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOJavaScriptLink")]
    [CmdletHelp("Returns all or a specific custom action(s) with location type ScriptLink")]
    public class GetJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "Name of the Javascript link. Omit this parameter to retrieve all script links")]
        [Alias("Key")]
        public string Name = string.Empty;

        [Parameter(Mandatory = false, HelpMessage = "Scope of the action, either Web (default) or Site")]
        public CustomActionScope Scope = CustomActionScope.Web;

        protected override void ExecuteCmdlet()
        {
            IEnumerable<UserCustomAction> actions = null;

            if (Scope == CustomActionScope.Web)
            {
                actions = SelectedWeb.GetCustomActions().Where(c => c.Location == "ScriptLink");
            }
            else
            {
                actions = ClientContext.Site.GetCustomActions().Where(c => c.Location == "ScriptLink");
            }

            if (!string.IsNullOrEmpty(Name))
            {
                var foundAction = actions.FirstOrDefault(x => x.Name == Name);
                WriteObject(foundAction, true);
            }
            else
            {
                WriteObject(actions,true);
            }
        }
    }
}
