using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

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
