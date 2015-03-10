using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Resources = OfficeDevPnP.PowerShell.Commands.Properties.Resources;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOJavaScriptLink", SupportsShouldProcess = true)]
    [CmdletHelp("Removes a JavaScript link or block from a web or sitecollection")]
    public class RemoveJavaScriptLink : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline=true,Position=0)]
        public string Key = string.Empty;

        [Parameter(Mandatory = false)]
        public SwitchParameter FromSite;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (Force || ShouldContinue(Resources.RemoveJavaScript, Resources.Confirm))
            {
                if (!FromSite)
                {
                    SelectedWeb.DeleteJsLink(Key);
                }
                else
                {
                    var site = ClientContext.Site;
                    site.DeleteJsLink(Key);
                }
            }
        }
    }
}
