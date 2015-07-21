using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWebPartProperty")]
    [CmdletHelp("Sets a web part property", Category = "Web Parts")]
    public class SetWebPartProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public PSObject Value = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (Value.BaseObject is string)
            {
                SelectedWeb.SetWebPartProperty(Key, Value.ToString(), Identity.Id, PageUrl);
            }
            else if (Value.BaseObject is int)
            {
                SelectedWeb.SetWebPartProperty(Key, (int)Value.BaseObject, Identity.Id, PageUrl);
            }
        }
    }
}
