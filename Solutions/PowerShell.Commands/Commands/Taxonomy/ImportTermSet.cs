using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using File = System.IO.File;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Import, "SPOTermSet", SupportsShouldProcess = false)]
    [CmdletHelp("Imports a taxonomy term set from a file in the standard format.", Category = "Taxonomy",
        DetailedDescription = @"The format of the file is the same as that used by the import function in the web interface. A sample file can be obtained from the web interface.

This is a CSV file, with the following headings:

  Term Set Name,Term Set Description,LCID,Available for Tagging,Term Description,Level 1 Term,Level 2 Term,Level 3 Term,Level 4 Term,Level 5 Term,Level 6 Term,Level 7 Term

The first data row must contain the Term Set Name, Term Set Description, and LCID, and should also contain the first term. 

It is recommended that a fixed GUID be used as the termSetId, to allow the term set to be easily updated (so do not pass Guid.Empty).

In contrast to the web interface import, this is not a one-off import but runs synchronisation logic allowing updating of an existing Term Set. When synchronising, any existing terms are matched (with Term Description and Available for Tagging updated as necessary), any new terms are added in the correct place in the hierarchy, and (if synchroniseDeletions is set) any terms not in the imported file are removed.

The import file also supports an expanded syntax for the Term Set Name and term names (Level 1 Term, Level 2 Term, etc). These columns support values with the format 'Name | GUID', with the name and GUID separated by a pipe character (note that the pipe character is invalid to use within a taxomony item name). This expanded syntax is not required, but can be used to ensure all terms have fixed IDs.")]
    [CmdletExample(
        Code = @"PS:> Import-SPOTermSet -GroupName 'Standard Terms' -Path 'C:\\Temp\\ImportTermSet.csv' -SynchronizeDeletions",
        Remarks = "Creates (or updates) the term set specified in the import file, in the group specified, removing any existing terms not in the file.",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Import-SPOTermSet -TermStoreName 'My Term Store' -GroupName 'Standard Terms' -Path 'C:\\Temp\\ImportTermSet.csv' -TermSetId '{15A98DB6-D8E2-43E6-8771-066C1EC2B8D8}' ",
        Remarks = "Creates (or updates) the term set specified in the import file, in the term store and group specified, using the specified ID.",
        SortOrder = 2)]
    [CmdletExample(
        Code = @"PS:> Import-SPOTermSet -GroupName 'Standard Terms' -Path 'C:\\Temp\\ImportTermSet.csv' -IsOpen $true -Contact 'user@example.org' -Owner 'user@example.org'",
        Remarks = "Creates (or updates) the term set specified in the import file, setting the IsOpen, Contact, and Owner properties as specified.",
        SortOrder = 3)]
    public class ImportTermSet : SPOCmdlet
    {
        [Parameter(Mandatory = true, 
            HelpMessage = "Group to import the term set to; an error is returned if the group does not exist.")]
        public string GroupName;

        [Parameter(Mandatory = true, 
            HelpMessage = "Local path to the file containing the term set to import, in the standard format (as the 'sample import file' available in the Term Store Administration).")]
        public string Path;

        [Parameter(Mandatory = false, 
            HelpMessage = "GUID to use for the term set; if not specified, or the empty GUID, a random GUID is generated and used.")]
        public Guid TermSetId = default(Guid);

        [Parameter(Mandatory = false, 
            HelpMessage = "If specified, the import will remove any terms (and children) previously in the term set but not in the import file; default is to leave them.")]
        public SwitchParameter SynchronizeDeletions;

        [Parameter(Mandatory = false, 
            HelpMessage = "Whether the term set should be marked open; if not specified, then the existing setting is not changed.")]
        public bool? IsOpen;

        [Parameter(Mandatory = false, 
            HelpMessage = "Contact for the term set; if not specified, the existing setting is retained.")]
        public string Contact;

        [Parameter(Mandatory = false, 
            HelpMessage = "Owner for the term set; if not specified, the existing setting is retained.")]
        public string Owner;

        [Parameter(Mandatory = false, 
            HelpMessage = "Term store to import into; if not specified the default term store is used.")]
        public string TermStoreName;

        protected override void ExecuteCmdlet()
        {
            if (!System.IO.Path.IsPathRooted(Path))
            {
                Path = System.IO.Path.Combine(SessionState.Path.CurrentFileSystemLocation.Path, Path);
            }

            var taxonomySession = TaxonomySession.GetTaxonomySession(ClientContext);
            // Get Term Store
            var termStore = default(TermStore);
            if (string.IsNullOrEmpty(TermStoreName))
            {
                termStore = taxonomySession.GetDefaultSiteCollectionTermStore();
            }
            else
            {
                termStore = taxonomySession.TermStores.GetByName(TermStoreName);
            }
            // Get Group
            var group = termStore.GetTermGroupByName(GroupName);
            // Import
            var termSet = default(TermSet);
            if (group != null) {
                termSet = group.ImportTermSet(Path, TermSetId, SynchronizeDeletions, IsOpen, Contact, Owner);
            }
            else
            {
                throw new Exception("Group does not exist.");
            }

            WriteObject(termSet);
        }
    }
}
