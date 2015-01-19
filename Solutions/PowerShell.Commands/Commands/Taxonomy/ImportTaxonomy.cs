using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOTaxonomy", SupportsShouldProcess = true)]
    [CmdletHelp("Imports a taxonomy from either a string array or a file")]
    [CmdletExample(Code = @"
PS:> Import-SPOTaxonomy -Terms 'Company|Locations|Stockholm'",
           Remarks = "Creates a new termgroup, 'Company', a termset 'Locations' and a term 'Stockholm'")]
    [CmdletExample(Code = @"
PS:> Import-SPOTaxonomy -Terms 'Company|Locations|Stockholm|Central','Company|Locations|Stockholm|North'",
       Remarks = "Creates a new termgroup, 'Company', a termset 'Locations', a term 'Stockholm' and two subterms: 'Central', and 'North'")]
    public class ImportTaxonomy : SPOCmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "Direct", HelpMessage = "An array of strings describing termgroup, termset, term, subterms using a default delimiter of '|'.")]
        public string[] Terms;

        [Parameter(Mandatory = true, ParameterSetName = "File", HelpMessage = "Specifies a file containing terms per line, in the format as required by the Terms parameter.")]
        public string Path;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public int LCID = 1033;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string TermStoreName;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string Delimiter = "|";

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "If specified, existing terms will be overwritten. Notice that this only works if you include term ids in your import data.")]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            string[] lines = null;
            if (ParameterSetName == "File")
            {
                lines = System.IO.File.ReadAllLines(Path);
            }
            else
            {
                lines = Terms;
            }
            if (!string.IsNullOrEmpty(TermStoreName))
            {
                var taxSession = TaxonomySession.GetTaxonomySession(ClientContext);
                var termStore = taxSession.TermStores.GetByName(TermStoreName);
                ClientContext.Site.ImportTerms(lines, LCID, termStore, Delimiter, Force);
            }
            else
            {
                ClientContext.Site.ImportTerms(lines, LCID, Delimiter, Force);
            }
        }

    }
}
