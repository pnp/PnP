using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOSite")]
    [CmdletHelp("Returns the current site collection from the context.")]
    public class GetSite : SPOCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            var site = ClientContext.Site;
            ClientContext.Load(site, s => s.Url, s => s.CompatibilityLevel);
            ClientContext.ExecuteQuery();
            WriteObject(site);
        }
    }

}
