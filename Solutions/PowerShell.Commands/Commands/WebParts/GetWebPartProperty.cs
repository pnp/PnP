using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

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
            var properties = SelectedWeb.GetWebPartProperties(Identity.Id, PageUrl);
            WriteObject(properties.FieldValues);
        }



    }
}
