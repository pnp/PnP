using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSite")]
    [CmdletHelp("Returns the current site collection from the context.")]
    public class GetSite : SPOCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var site = ClientContext.Site;
            ClientContext.Load(site);
            WriteObject(new SPOSite(site));
        }
    }

}
