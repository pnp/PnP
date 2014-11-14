using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavascriptBlock")]
    [CmdletHelp("Adds a link to a JavaScript snippet/block to a web")]
    public class AddJavaScriptBlock : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public string Script = null;
       
        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.AddJsBlock(Key,Script);
        }
    }
}
