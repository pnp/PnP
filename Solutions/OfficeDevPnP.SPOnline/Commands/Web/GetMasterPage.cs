using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOMasterPage")]
    public class GetMasterPage : SPOWebCmdlet
    {

        protected override void ExecuteCmdlet()
        {
            WriteObject(SPOnline.Core.SPOWeb.GetMasterPage(this.SelectedWeb, ClientContext));
        }
    }
}
