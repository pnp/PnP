using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System.Management.Automation;
using System;
using OfficeDevPnP.Core.Entities;
using Microsoft.SharePoint.Client.Taxonomy;
using System.Collections.Generic;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using System.Linq;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Set, "SPODefaultColumnValues")]
    [CmdletHelp("Sets default column values for a document library", DetailedDescription="Sets default column values for a document library, per folder, or for the root folder if the folder parameter has not been specified. Supports both text and taxonomy fields.")]
    [CmdletExample(Code = "PS:> Set-SPODefaultColumnValues -List Documents -Field TaxKeyword -Value \"Company|Locations|Stockholm\"", SortOrder = 1, Remarks = "Sets a default value for the enterprise keywords field on a library to a term called \"Stockholm\", located in the \"Locations\" term set, which is part of the \"Company\" term group")]
    [CmdletExample(Code = "PS:> Set-SPODefaultColumnValues -List Documents -Field MyTextField -Value \"DefaultValue\"", SortOrder = 2, Remarks = "Sets a default value for the MyTextField text field on a library to a value of \"DefaultValue\"")]
    public class SetDefaultColumnValues : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 0, HelpMessage = "The ID, Name or Url of the list.")]
        public ListPipeBind List;

        [Parameter(Mandatory = true, HelpMessage="The internal name, id or a reference to a field")]
        public FieldPipeBind Field;

        [Parameter(Mandatory = true, HelpMessage="A list of values. In case of a text field the values will be concatenated, separated by a semi-column. In case of a taxonomy field multiple values will added")]
        public string[] Value;

        [Parameter(Mandatory = false, HelpMessage="A library relative folder path, if not specified it will set the default column values on the root folder of the library ('/')")]
        public string Folder = "/";

        protected override void ExecuteCmdlet()
        {
            List list = null;
            if (List != null)
            {
                list = SelectedWeb.GetList(List);
            }
            if (list != null)
            {
                if (list.BaseTemplate == (int)ListTemplateType.DocumentLibrary)
                {
                    Field field = null;
                    // Get the field
                    if (Field.Field != null)
                    {
                        field = Field.Field;
                        if (!field.IsPropertyAvailable("TypeAsString"))
                        {
                            ClientContext.Load(field, f => f.TypeAsString);
                        }
                        if (!field.IsPropertyAvailable("InternalName"))
                        {
                            ClientContext.Load(field, f => f.InternalName);
                        }
                        ClientContext.Load(field);
                        ClientContext.ExecuteQueryRetry();
                    }
                    else if (Field.Id != Guid.Empty)
                    {
                        field = list.Fields.GetById(Field.Id);
                        ClientContext.Load(field, f => f.InternalName, f => f.TypeAsString);
                        ClientContext.ExecuteQueryRetry();
                    }
                    else if (!string.IsNullOrEmpty(Field.Name))
                    {
                        field = list.Fields.GetByInternalNameOrTitle(Field.Name);
                        ClientContext.Load(field, f => f.InternalName, f => f.TypeAsString);
                        ClientContext.ExecuteQueryRetry();
                    }
                    if (field != null)
                    {
                        IDefaultColumnValue defaultColumnValue = null;
                        if (field.TypeAsString == "Text")
                        {
                            var values = string.Join(";", Value);
                            defaultColumnValue = new DefaultColumnTextValue()
                            {
                                FieldInternalName = field.InternalName,
                                FolderRelativePath = Folder,
                                Text = values
                            };
                        }
                        else
                        {
                            List<Term> terms = new List<Term>();

                            foreach (var termString in Value)
                            {
                                var term = ClientContext.Site.GetTaxonomyItemByPath(termString);
                                if (term != null)
                                {
                                    terms.Add(term as Term);
                                }
                            }
                            if (terms.Any())
                            {
                                defaultColumnValue = new DefaultColumnTermValue()
                                {
                                    FieldInternalName = field.InternalName,
                                    FolderRelativePath = Folder,
                                };
                                terms.ForEach(t => ((DefaultColumnTermValue)defaultColumnValue).Terms.Add(t));
                            }
                        }
                        list.SetDefaultColumnValues(new List<IDefaultColumnValue>() { defaultColumnValue });
                    }
                }
                else
                {
                    WriteWarning("List is not a document library");
                }

            }
        }
    }

}
