using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWeb")]
    [CmdletHelp("Sets properties on a web")]
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
            if (!string.IsNullOrEmpty(SiteLogoUrl))
            {
                this.SelectedWeb.SiteLogoUrl = SiteLogoUrl;
                this.SelectedWeb.Update();
            }
            if (!string.IsNullOrEmpty(AlternateCssUrl))
            {
                this.SelectedWeb.AlternateCssUrl = AlternateCssUrl;
                this.SelectedWeb.Update();
            }
            if(!string.IsNullOrEmpty(Title))
            {
                this.SelectedWeb.Title = Title;
                this.SelectedWeb.Update();
            }
            ClientContext.ExecuteQuery();
        }
    }

}
