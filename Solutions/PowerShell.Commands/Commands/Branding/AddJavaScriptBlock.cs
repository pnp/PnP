using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOJavaScriptBlock")]
    [CmdletHelp("Adds a link to a JavaScript snippet/block to a web or site collection", DetailedDescription = "Specify a scope as 'Site' to add the custom action to all sites in a site collection.")]
    public class AddJavaScriptBlock : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("Key")]
        public string Name = string.Empty;

        [Parameter(Mandatory = true)]
        public string Script = null;

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
                SelectedWeb.AddJsBlock(Name, Script, Sequence);
            }
            else
            {
                var site = ClientContext.Site;
                site.AddJsBlock(Name, Script, Sequence);
            }
        }
    }
}
