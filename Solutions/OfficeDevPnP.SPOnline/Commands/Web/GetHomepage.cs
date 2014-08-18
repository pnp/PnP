using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOHomePage")]
    public class GetHomePage : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(SPOnline.Core.SPOWeb.GetHomePage(this.SelectedWeb, ClientContext));
        }
    }
}
