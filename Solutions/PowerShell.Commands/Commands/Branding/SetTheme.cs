using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOTheme")]
    public class SetTheme : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public string ColorPaletteUrl = null;

        [Parameter(Mandatory = false)]
        public string FontSchemeUrl = null;

        [Parameter(Mandatory = false)]
        public string BackgroundImageUrl = null;

        [Parameter(Mandatory = false)]
        public SwitchParameter ShareGenerated = false;

        protected override void ExecuteCmdlet()
        {
            SelectedWeb.ApplyTheme(ColorPaletteUrl, FontSchemeUrl, BackgroundImageUrl, ShareGenerated);
            ClientContext.ExecuteQueryRetry();
        }
    }
}
