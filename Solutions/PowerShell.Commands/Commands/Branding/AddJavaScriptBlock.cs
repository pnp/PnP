using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavascriptBlock")]
    [CmdletHelp("Adds a link to a JavaScript snippet/block to a web or site collection")]
    public class AddJavaScriptBlock : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public string Script = null;

        [Parameter(Mandatory = false)]
        public int Sequence = 0;

        [Parameter(Mandatory = false)]
        [Alias("AddToSite")]
        public SwitchParameter SiteScoped;

        protected override void ExecuteCmdlet()
        {
            if (!SiteScoped)
            {
                SelectedWeb.AddJsBlock(Key, Script, Sequence);
            }
            else
            {
                var site = ClientContext.Site;
                site.AddJsBlock(Key, Script, Sequence);
            }
        }
    }
}
