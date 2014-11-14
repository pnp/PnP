using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOJavaScriptLink")]
    [CmdletHelp("Removes a JavaScript link or block from a web")]
    public class RemoveJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline=true,Position=0)]
        public string Key = string.Empty;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Force || ShouldContinue(Properties.Resources.RemoveJavaScript, Properties.Resources.Confirm))
            {
                this.SelectedWeb.DeleteJsLink(Key);
            }
        }
    }
}
