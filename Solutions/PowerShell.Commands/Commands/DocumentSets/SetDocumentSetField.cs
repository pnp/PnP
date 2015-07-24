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
    [Cmdlet(VerbsCommon.Set, "SPODocumentSetField")]
    [CmdletHelp("Sets a site column from the avaiable content types to a document set", Category = "Document Sets")]
    [CmdletExample(
        Code = @"PS:> Set-SPODocumentSetField -Field ""Test Field"" -DocumentSet ""Test Document Set"" -SetAsSharedField -SetAsWelcomePageField",
        Remarks = "This will set the field, available in one the available content types, as a Shared Field and as a Welcome Page Field.",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Set-SPODocumentSetField -Field ""Test Field"" -DocumentSet ""Test Document Set"" -RemoveAsSharedField -RemoveAsWelcomePageField",
        Remarks = "This will remove the field, available in one the available content types, as a Shared Field and as a Welcome Page Field.",
        SortOrder = 1)]
    public class SetFieldInDocumentSet : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, HelpMessage = "The document set to set the field in. Either specify a name, a document set template object, an id, or a content type object")]
        public DocumentSetPipeBind DocumentSet;

        [Parameter(Mandatory = true, HelpMessage = "The field to set. The field needs to be available in one of the available content types. Either specify a name, an id or a field object")]
        public FieldPipeBind Field; 

        [Parameter(Mandatory = false, HelpMessage = "Set the field as a Shared Field")]
        public SwitchParameter SetSharedField;

        [Parameter(Mandatory = false, HelpMessage = "Set the field as a Welcome Page field")]
        public SwitchParameter SetWelcomePageField;

        [Parameter(Mandatory = false, HelpMessage = "Removes the field as a Shared Field")]
        public SwitchParameter RemoveSharedField;

        [Parameter(Mandatory = false, HelpMessage = "Removes the field as a Welcome Page Field")]
        public SwitchParameter RemoveWelcomePageField;

        protected override void ExecuteCmdlet()
        {
            if (this.MyInvocation.BoundParameters.ContainsKey("SetSharedField") && this.MyInvocation.BoundParameters.ContainsKey("RemoveSharedField"))
            {
                WriteWarning("Cannot set and remove a shared field at the same time");
                return;
            }
            if (this.MyInvocation.BoundParameters.ContainsKey("SetWelcomePageField") && this.MyInvocation.BoundParameters.ContainsKey("RemoveWelcomePageField"))
            {
                WriteWarning("Cannot set and remove a welcome page field at the same time");
                return;
            }
         
            var docSetTemplate = DocumentSet.GetDocumentSetTemplate(SelectedWeb);

            ClientContext.Load(docSetTemplate, dt => dt.AllowedContentTypes, dt => dt.SharedFields, dt => dt.WelcomePageFields);
            ClientContext.ExecuteQueryRetry();

            var field = Field.Field;
            if (field == null)
            {
                if (Field.Id != Guid.Empty)
                {
                    field = SelectedWeb.Fields.GetById(Field.Id);
                }
                else if (!string.IsNullOrEmpty(Field.Name))
                {
                    field = SelectedWeb.Fields.GetByInternalNameOrTitle(Field.Name);
                }
                ClientContext.Load(field);
                ClientContext.ExecuteQueryRetry();
            }
            if (field != null)
            {
                // Check if field is part of the content types in the document set
                Field existingField = null;
                foreach (var allowedCtId in docSetTemplate.AllowedContentTypes)
                {
                    var allowedCt = SelectedWeb.GetContentTypeById(allowedCtId.StringValue, true);

                    var fields = allowedCt.Fields;
                    ClientContext.Load(fields, fs => fs.Include(f => f.Id));
                    ClientContext.ExecuteQueryRetry();
                    existingField = fields.FirstOrDefault(f => f.Id == field.Id);
                    if (existingField != null)
                    {
                        break;
                    }
                }
                if (existingField != null)
                {
                    if (SetSharedField)
                    {
                        docSetTemplate.SharedFields.Add(field);
                    }
                    if (SetWelcomePageField)
                    {
                        docSetTemplate.WelcomePageFields.Add(field);
                    }
                    if (RemoveSharedField)
                    {
                        docSetTemplate.SharedFields.Remove(field);
                    }
                    if (RemoveWelcomePageField)
                    {
                        docSetTemplate.WelcomePageFields.Remove(field.Id);
                    }
                    docSetTemplate.Update(true);
                    ClientContext.ExecuteQueryRetry();
                }
                else
                {
                    WriteWarning("Field not present in document set allowed content types");
                }
            }
        }
    }
}
