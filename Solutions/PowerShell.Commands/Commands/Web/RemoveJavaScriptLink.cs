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
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.DeleteJsLink(Key);
        }
    }
}
