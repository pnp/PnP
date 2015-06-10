using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOTheme")]
    [CmdletHelp("Sets the theme of the current web.", Category = "Branding")]
    [CmdletExample(
        Code = @"PS:> Set-SPOTheme -ColorPaletteUrl /_catalogs/theme/15/company.spcolor",
        SortOrder = 1)]
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
