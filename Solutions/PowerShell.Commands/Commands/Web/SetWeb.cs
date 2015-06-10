using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWeb")]
    [CmdletHelp("Sets properties on a web", Category = "Webs")]
    public class SetWeb : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public string SiteLogoUrl;

        [Parameter(Mandatory = false)]
        public string AlternateCssUrl;

        [Parameter(Mandatory = false)]
        public string Title;

        protected override void ExecuteCmdlet()
        {
            if (SiteLogoUrl != null)
            {
                SelectedWeb.SiteLogoUrl = SiteLogoUrl;
                SelectedWeb.Update();
            }
            if (!string.IsNullOrEmpty(AlternateCssUrl))
            {
                SelectedWeb.AlternateCssUrl = AlternateCssUrl;
                SelectedWeb.Update();
            }
            if(!string.IsNullOrEmpty(Title))
            {
                SelectedWeb.Title = Title;
                SelectedWeb.Update();
            }
            ClientContext.ExecuteQueryRetry();
        }
    }

}
