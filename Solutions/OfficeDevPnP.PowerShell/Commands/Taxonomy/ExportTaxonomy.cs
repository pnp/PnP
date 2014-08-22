using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.Core;

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
                exportedTerms = SPOTaxonomy.ExportTermSet(TermSetId.Id, IncludeID, ClientContext, Delimiter);
            }
            else
            {
                exportedTerms = SPOTaxonomy.ExportAllTerms(IncludeID, ClientContext, Delimiter);
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
