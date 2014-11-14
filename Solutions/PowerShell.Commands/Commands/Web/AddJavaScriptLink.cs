using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.Core.Entities;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavascriptLink")]
    [CmdletHelp("Adds a link to a JavaScript file to a web")]
    public class AddJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public string[] Url = null;
       
        protected override void ExecuteCmdlet()
        {
            this.SelectedWeb.AddJsLink(Key, Url);
        }
    }
}
