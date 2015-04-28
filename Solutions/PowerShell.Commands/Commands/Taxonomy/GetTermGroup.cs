using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using Microsoft.SharePoint.Client.Taxonomy;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomyTermGroup", SupportsShouldProcess = false)]
    [CmdletHelp(@"Returns a taxonomy term group",Category = "Taxonomy")]
    public class GetTermGroup : SPOCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, 
            HelpMessage = "Name of the taxonomy term group to retrieve.")]
        public string GroupName;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets,
            HelpMessage = "Term store to check; if not specified the default term store is used.")]
        public string TermStoreName;

        protected override void ExecuteCmdlet()
        {
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

            WriteObject(group);
        }

    }
}
