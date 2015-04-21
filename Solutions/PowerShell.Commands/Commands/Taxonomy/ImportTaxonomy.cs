using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOTaxonomy", SupportsShouldProcess = true)]
    [CmdletHelp("Imports a taxonomy from either a string array or a file", Category = "Taxonomy")]
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
        public int Lcid = 1033;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string TermStoreName;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string Delimiter = "|";

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets, HelpMessage = "If specified, terms that exist in the termset, but are not in the imported data will be removed.")]
        public SwitchParameter SynchronizeDeletions;

        protected override void ExecuteCmdlet()
        {

            string[] lines;
            if (ParameterSetName == "File")
            {
                if (!System.IO.Path.IsPathRooted(Path))
                {
                    Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
                }

                lines = File.ReadAllLines(Path);
            }
            else
            {
                lines = Terms;
            }
            if (!string.IsNullOrEmpty(TermStoreName))
            {
                var taxSession = TaxonomySession.GetTaxonomySession(ClientContext);
                var termStore = taxSession.TermStores.GetByName(TermStoreName);
                ClientContext.Site.ImportTerms(lines, Lcid, termStore, Delimiter, SynchronizeDeletions);
            }
            else
            {
                ClientContext.Site.ImportTerms(lines, Lcid, Delimiter, SynchronizeDeletions);
            }
        }

    }
}
