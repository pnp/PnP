using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOWebPartProperty")]
    public class GetWebPartProperty : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public string PageUrl = string.Empty;

        [Parameter(Mandatory = true)]
        public GuidPipeBind Identity;

        protected override void ExecuteCmdlet()
        {
            var properties = this.SelectedWeb.GetWebPartProperties(Identity.Id, PageUrl);
            WriteObject(properties.FieldValues);
        }



    }
}
