using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

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
            SelectedWeb.AddJsLink(Key, Url);
        }
    }
}
