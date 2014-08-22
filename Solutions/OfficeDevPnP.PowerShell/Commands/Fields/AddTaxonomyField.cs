using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.PowerShell.Core;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Add, "SPOTaxonomyField")]
    public class AddTaxonomyField : SPOWebCmdlet
    {
        [Parameter(Mandatory=true)]
        public SPOListPipeBind List;

        [Parameter(Mandatory = true)]
        public string DisplayName;

        [Parameter(Mandatory = true)]
        public string InternalName;

        [Parameter(Mandatory = true)]
        public string TermSetPath;

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
            var list = this.SelectedWeb.GetList(List);

            Guid id = Id.Id;
            if(id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }

            var termStore = (TermStore) OfficeDevPnP.PowerShell.Core.SPOTaxonomy.GetDefaultKeywordsTermStore(ClientContext);
            var termSet = (TermSet) OfficeDevPnP.PowerShell.Core.SPOTaxonomy.GetTaxonomyItemByPath(TermSetPath, ClientContext);
            OfficeDevPnP.PowerShell.Core.SPOField.AddTaxonomyField(list, DisplayName, InternalName, Group, termStore, termSet, id, Required, AddToDefaultView, MultiValue, ClientContext);
        }

    }

}
