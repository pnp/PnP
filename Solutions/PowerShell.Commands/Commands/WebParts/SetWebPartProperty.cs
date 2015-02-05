using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPOWebPartProperty")]
    public class SetWebPartProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true)]
        public string Key = string.Empty;

        [Parameter(Mandatory = true)]
        public object Value = string.Empty;

        protected override void ExecuteCmdlet()
        {
            var stringValue = Value as string;
            if (stringValue != null)
            {
                SelectedWeb.SetWebPartProperty(Key, stringValue, Identity.Id, PageUrl);
            }
            else if (Value is int)
            {
                SelectedWeb.SetWebPartProperty(Key, (int)Value, Identity.Id, PageUrl);
            }
        }
    }
}
