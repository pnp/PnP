using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOTaxonomyField")]
    public class AddTaxonomyField : SPOWebCmdlet
    {
        [Parameter(Mandatory = false)]
        public SPOListPipeBind List;

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
            Field field = null;
            var termSet = ClientContext.Site.GetTaxonomyItemByPath(TermSetPath, TermPathDelimiter);
            Guid id = Id.Id;
            if (id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }

            if (List != null)
            {
                var list = this.SelectedWeb.GetList(List);

                field = list.CreateTaxonomyField(id, InternalName, DisplayName, Group, termSet as TermSet, MultiValue);
            }
            else
            {
                field = this.SelectedWeb.CreateTaxonomyField(id, InternalName, DisplayName, Group, termSet as TermSet, MultiValue);
            }
            WriteObject(field);
        }

    }

}
