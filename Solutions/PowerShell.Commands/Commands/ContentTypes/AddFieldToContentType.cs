using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{

    [Cmdlet(VerbsCommon.Add, "SPOFieldToContentType")]
    [CmdletHelp("Adds an existing site column to a content type")]
    [CmdletExample(
     Code = @"PS:> Add-SPOFieldToContentType -Field ""Project_Name"" -ContentType ""Project Document""",
     Remarks = @"This will add an existing site column with an internal name of ""Project_Name"" to a content type called ""Project Document""", SortOrder = 1)]
    public class AddFieldToContentType : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public FieldPipeBind Field;

        [Parameter(Mandatory = true)]
        public ContentTypePipeBind ContentType;

        [Parameter(Mandatory = false)]
        public SwitchParameter Required;

        [Parameter(Mandatory = false)]
        public SwitchParameter Hidden;

        protected override void ExecuteCmdlet()
        {
            Field field = Field.Field;
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
                if (ContentType.ContentType != null)
                {
                    SelectedWeb.AddFieldToContentType(ContentType.ContentType, field, Required, Hidden);
                }
                else
                {
                    ContentType ct = null;
                    if (!string.IsNullOrEmpty(ContentType.Id))
                    {
                        ct = SelectedWeb.GetContentTypeById(ContentType.Id);
                      
                    }
                    else
                    {
                        ct = SelectedWeb.GetContentTypeByName(ContentType.Name);
                    }
                    if (ct != null)
                    {
                        SelectedWeb.AddFieldToContentType(ct, field, Required, false);
                    }
                }
            }
            else
            {
                throw new Exception("Field not found");
            }
        }


    }
}
