using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System.Linq;


namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWebPart")]
    public class RemoveWebPart : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = "ID")]
        public GuidPipeBind Identity;

        [Parameter(Mandatory = true, ParameterSetName = "NAME")]
        public string Name = string.Empty;

        [Parameter(Mandatory = true, ParameterSetName = "ID")]
        [Parameter(Mandatory = true, ParameterSetName = "NAME")]
        public string PageUrl = string.Empty;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "NAME")
            {
                SelectedWeb.DeleteWebPart(PageUrl, Name);
            }
            else
            {
                var wps = SelectedWeb.GetWebParts(PageUrl);
                var wp = from w in wps where w.Id == Identity.Id select w;
                if(wp.Any())
                {
                    wp.FirstOrDefault().DeleteWebPart();
                    ClientContext.ExecuteQueryRetry();
                }
            }
        }
    }
}
