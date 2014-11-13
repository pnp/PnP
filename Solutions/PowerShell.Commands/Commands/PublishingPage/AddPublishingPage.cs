using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using Microsoft.SharePoint.Client.WebParts;
using OfficeDevPnP.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOPublishingPage")]
    public class AddPublishingPage : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        [Alias("Name")]
        public string PageName = string.Empty;

        [Parameter(Mandatory = true)]
        public string PageTemplateName = null;

        [Parameter(Mandatory = false, ParameterSetName = "WithTitle")]
        public string Title;

        protected override void ExecuteCmdlet()
        {
            switch (ParameterSetName)
            {
                case "WithTitle":
                    {
                        this.SelectedWeb.AddPublishingPage(PageName, PageTemplateName, Title);
                        break;
                    }
                default:
                    {
                        this.SelectedWeb.AddPublishingPage(PageName, PageTemplateName);
                        break;
                    }
            }
        }
    }
}
