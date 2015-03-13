using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Enums;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOJavaScriptLink", SupportsShouldProcess = true)]
    [CmdletHelp("Removes a JavaScript link or block from a web or sitecollection", Category = "Branding")]
    public class RemoveJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0, HelpMessage = "Name of the Javascript link. Omit this parameter to retrieve all script links")]
        [Alias("Key")]
        public string Name = string.Empty;

        [Parameter(Mandatory = false, DontShow = true)]
        public SwitchParameter FromSite;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public CustomActionScope Scope = CustomActionScope.Web;

        protected override void ExecuteCmdlet()
        {

            // Following code to handle desprecated parameter
            CustomActionScope setScope;

            if (MyInvocation.BoundParameters.ContainsKey("FromSite"))
            {
                setScope = CustomActionScope.Site;
            }
            else
            {
                setScope = Scope;
            }

            var action = !FromSite ? SelectedWeb.GetCustomActions().FirstOrDefault(c => c.Name == Name) : ClientContext.Site.GetCustomActions().FirstOrDefault(c => c.Name == Name);
            if (action != null)
            {
                if (Force || ShouldContinue(string.Format(Resources.RemoveJavaScript0,action.Name), Resources.Confirm))
                {
                    if (setScope == CustomActionScope.Web)
                    {
                        SelectedWeb.DeleteJsLink(Name);
                    }
                    else
                    {
                        var site = ClientContext.Site;
                        site.DeleteJsLink(Name);
                    }
                }
            }
        }
    }
}
