using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.New, "SPOWeb")]
    public class NewWeb : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string Title;

        [Parameter(Mandatory = true)]
        public string Url;

        [Parameter(Mandatory = false)]
        public string Description = string.Empty;

        [Parameter(Mandatory = false)]
        public int Locale = 1033;

        [Parameter(Mandatory = true)]
        public string Template = string.Empty;

        [Parameter(Mandatory = false)]
        public SwitchParameter BreakInheritance = false;

        protected override void ExecuteCmdlet()
        {
            PowerShell.Core.SPOWeb.CreateWeb(Url, Title, Locale, Description, Template, this.SelectedWeb, ClientContext, !BreakInheritance);

            WriteVerbose(string.Format(Properties.Resources.Web0CreatedAt1, Title, Url));

        }

    }
}
