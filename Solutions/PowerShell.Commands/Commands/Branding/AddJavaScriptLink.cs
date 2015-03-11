using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavaScriptLink")]
    [CmdletHelp("Adds a link to a JavaScript file to a web or sitecollection")]
    public class AddJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public string[] Url = null;

        [Parameter(Mandatory = false)]
        public int Sequence = 0;

        [Parameter(Mandatory = false, DontShow = true)]
        [Alias("AddToSite")]
        public SwitchParameter SiteScoped;

        [Parameter(Mandatory = false)]
        public CustomActionScope Scope = CustomActionScope.Web;

        protected override void ExecuteCmdlet()
        {
            // Following code to handle desprecated parameter
            CustomActionScope setScope;

            if (MyInvocation.BoundParameters.ContainsKey("SiteScoped"))
            {
                setScope = CustomActionScope.Site;
            }
            else
            {
                setScope = Scope;
            }

            if (setScope == CustomActionScope.Web)
            {
                SelectedWeb.AddJsLink(Key, Url, Sequence);
            }
            else
            {
                var site = ClientContext.Site;
                site.AddJsLink(Key, Url, Sequence);
            }
        }
    }
}
