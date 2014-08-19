using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;


namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWebPart")]
    public class RemoveWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ID")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true, ParameterSetName = "TITLE")]
        public string Title = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "ID")]
        [Parameter(Mandatory = true, ParameterSetName = "TITLE")]
        public string PageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "TITLE")
            {
                SPOnline.Core.SPOWebParts.RemoveWebPartByTitle(PageUrl, Title, this.SelectedWeb, ClientContext);
            }
            else
            {
                SPOnline.Core.SPOWebParts.RemoveWebPartById(PageUrl, Identity.Id, this.SelectedWeb, ClientContext);
            }
        }
    }
}
