using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOWebPart")]
    [CmdletHelp("Removes a webpart from a page", Category = "Web Parts")]
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
