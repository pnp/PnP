using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.DocumentSet;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.DocumentSets
{
    [Cmdlet(VerbsCommon.Get,"SPODocumentSetTemplate")]
    [CmdletHelp("Retrieves a document set template", Category = "Document Sets")]
    [CmdletExample(
        Code = @"PS:> Get-SPODocumentSetTemplate -Identity ""Test Document Set""",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPODocumentSetTemplate -Identity ""0x0120D520005DB65D094035A241BAC9AF083F825F3B""",
        SortOrder = 2)]
    public class GetDocumentSetTemplate : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, Position = 0, ValueFromPipeline = true, HelpMessage = "Either specify a name, an id, a document set template object or a content type object")]
        public DocumentSetPipeBind Identity;

        protected override void ExecuteCmdlet()
        { 
            var docSetTemplate = Identity.GetDocumentSetTemplate(SelectedWeb);

            ClientContext.Load(docSetTemplate, t => t.AllowedContentTypes, t => t.DefaultDocuments, t => t.SharedFields, t => t.WelcomePageFields);

            ClientContext.ExecuteQuery();

            WriteObject(docSetTemplate);
        }
    }
}
