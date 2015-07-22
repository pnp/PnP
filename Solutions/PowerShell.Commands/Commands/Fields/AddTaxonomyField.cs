using System;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOTaxonomyField")]
    [CmdletHelp("Adds a taxonomy field to a list or as a site column.", Category = "Fields")]
    public class AddTaxonomyField : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public ListPipeBind List;

        [Parameter(Mandatory = true)]
        public string DisplayName;

        [Parameter(Mandatory = true)]
        public string InternalName;

        [Parameter(Mandatory = true)]
        public string TermSetPath;

        [Parameter(Mandatory = false)]
        public string TermPathDelimiter = "|";

        [Parameter(Mandatory = false)]
        public string Group;

        [Parameter(Mandatory = false)]
        public GuidPipeBind Id = new GuidPipeBind();

        [Parameter(Mandatory = false)]
        public SwitchParameter AddToDefaultView;

        [Parameter(Mandatory = false)]
        public SwitchParameter MultiValue;

        [Parameter(Mandatory = false)]
        public SwitchParameter Required;

        [Parameter(Mandatory = false)]
        public AddFieldOptions FieldOptions = AddFieldOptions.DefaultValue;


        protected override void ExecuteCmdlet()
        {
            Field field;
            var termSet = ClientContext.Site.GetTaxonomyItemByPath(TermSetPath, TermPathDelimiter);
            Guid id = Id.Id;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }

            TaxonomyFieldCreationInformation fieldCI = new TaxonomyFieldCreationInformation()
            {
                Id = id,
                InternalName = InternalName,
                DisplayName = DisplayName,
                Group = Group,
                TaxonomyItem = termSet,
                MultiValue = MultiValue,
                Required = Required,
                AddToDefaultView = AddToDefaultView
            };

            if (List != null)
            {
                var list = List.GetList(SelectedWeb);

              

                field = list.CreateTaxonomyField(fieldCI);
            }
            else
            {
                field = SelectedWeb.CreateTaxonomyField(fieldCI);
            }
            WriteObject(field);
        }

    }

}
