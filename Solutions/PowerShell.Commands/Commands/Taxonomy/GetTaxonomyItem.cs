using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Core;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOTaxonomyItem", SupportsShouldProcess = true)]
    public class GetTaxonomyItem : SPOCmdlet
    {

        [Parameter(Mandatory = false, ValueFromPipeline = true, ParameterSetName = "Direct", HelpMessage = "An array of strings describing termgroup, termset, term, subterms using a default delimiter of '|'.")]
        public string Term;
        
        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public int LCID = 1033;

        [Parameter(Mandatory = false, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public string Delimiter = "|";

        protected override void ExecuteCmdlet()
        {

            WriteObject(ClientContext.Site.GetTaxonomyItemByPath(Term));
        }

    }
}
