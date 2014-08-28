using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using System.Text.RegularExpressions;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomySession")]
    public class GetTaxonomySession : SPOWebCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(ClientContext.Site.GetTaxonomySession());
        }

    }
}
