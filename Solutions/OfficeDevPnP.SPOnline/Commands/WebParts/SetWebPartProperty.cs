using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
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
        public string Value = string.Empty;

        protected override void ExecuteCmdlet()
        {
            SPOnline.Core.SPOWebParts.SetWebPartProperty(Key, Value, Identity.Id, PageUrl, this.SelectedWeb, ClientContext);
        }



    }
}
