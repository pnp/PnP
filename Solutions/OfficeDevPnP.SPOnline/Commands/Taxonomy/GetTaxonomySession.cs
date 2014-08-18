using OfficeDevPnP.SPOnline.Commands.Base;
using System.Management.Automation;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomySession")]
    public class GetTaxonomySession : SPOCmdlet
    {
        protected override void ExecuteCmdlet()
        {
            WriteObject(SPOnline.Core.SPOTaxonomy.GetTaxonomySession(ClientContext));
        }

    }
}
