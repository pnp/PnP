using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

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
            SelectedWeb.AddJsBlock(Key,Script);
        }
    }
}
