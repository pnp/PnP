using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomySession")]
    public class GetTaxonomySession : SPOCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(PowerShell.Core.SPOTaxonomy.GetTaxonomySession(ClientContext));
        }

    }
}
