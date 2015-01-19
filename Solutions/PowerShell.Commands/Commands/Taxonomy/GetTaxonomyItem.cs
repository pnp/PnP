using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomyItem", SupportsShouldProcess = true)]
    public class GetTaxonomyItem : SPOCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, HelpMessage = "The path, delimited by | of the taxonomy item to retrieve, alike GROUPLABEL|TERMSETLABEL|TERMLABEL")]
        public string Term;

        protected override void ExecuteCmdlet()
        {
            WriteObject(ClientContext.Site.GetTaxonomyItemByPath(Term));
        }

    }
}
