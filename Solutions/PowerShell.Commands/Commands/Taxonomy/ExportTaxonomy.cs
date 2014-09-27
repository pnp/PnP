using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsData.Export, "SPOTaxonomy", SupportsShouldProcess = true)]
    public class ExportTerms : SPOCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "TermSet")]
        public GuidPipeBind TermSetId = new GuidPipeBind();

        [Parameter(Mandatory = false)]
        public SwitchParameter IncludeID = false;

        [Parameter(Mandatory = false)]
        public string Path;

        [Parameter(Mandatory = false, ParameterSetName="TermSet")]
        public string TermStoreName;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        [Parameter(Mandatory = false)]
        public string Delimiter = "|";


        protected override void ExecuteCmdlet()
        {
            List<string> exportedTerms = null;
            if (ParameterSetName == "TermSet")
            {
                if (Delimiter != "|" && Delimiter == ";#")
                {
                    throw new Exception("Restricted delimiter specified");
                }
                if (!string.IsNullOrEmpty(TermStoreName))
                {
                    var taxSession = TaxonomySession.GetTaxonomySession(ClientContext);
                    var termStore = taxSession.TermStores.GetByName(TermStoreName);
                    exportedTerms = ClientContext.Site.ExportTermSet(TermSetId.Id, IncludeID, termStore, Delimiter);
                }
                else
                {
                    exportedTerms = ClientContext.Site.ExportTermSet(TermSetId.Id, IncludeID, Delimiter);
                }
            }
            else
            {
                exportedTerms = ClientContext.Site.ExportAllTerms(IncludeID, Delimiter);
            }

            if (Path == null)
            {
                WriteObject(exportedTerms);
            }
            else
            {
                if (System.IO.File.Exists(Path))
                {
                    if (Force || ShouldProcess(string.Format(Properties.Resources.File0ExistsOverwrite, Path), Properties.Resources.Confirm))
                    {
                        System.IO.File.WriteAllLines(Path, exportedTerms);
                    }
                }
                else
                {
                    System.IO.File.WriteAllLines(Path, exportedTerms);
                }
            }
        }

    }
}
