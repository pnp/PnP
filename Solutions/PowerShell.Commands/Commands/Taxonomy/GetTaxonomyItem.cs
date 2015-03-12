using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomyItem", SupportsShouldProcess = true)]
    [CmdletHelp(@"Returns a taxonomy item",Category = "Taxonomy")]
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
