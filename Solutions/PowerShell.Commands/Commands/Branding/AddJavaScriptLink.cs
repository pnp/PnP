using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavascriptLink")]
    [CmdletHelp("Adds a link to a JavaScript file to a web or sitecollection")]
    public class AddJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public string[] Url = null;

        [Parameter(Mandatory = false)]
        [Alias("AddToSite")]
        public SwitchParameter SiteScoped;

        protected override void ExecuteCmdlet()
        {
            if (!SiteScoped)
            {
                SelectedWeb.AddJsLink(Key, Url);
            }
            else
            {
                var site = ClientContext.Site;
                site.AddJsLink(Key, Url);
            }
        }
    }
}
