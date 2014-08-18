using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using Microsoft.SharePoint.Client.Taxonomy;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
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
            List list = null;
            if (List.List != null)
            {
                list = List.List;
            }
            else if (List.Id != Guid.Empty)
            {
                list = SPO.SPOList.GetListById(List.Id, SelectedWeb, ClientContext);
            }
            else if (!string.IsNullOrEmpty(List.Title))
            {
                list = SPO.SPOList.GetListByTitle(List.Title, SelectedWeb, ClientContext);
            }

            Guid id = Id.Id;
            if(id == Guid.Empty)
            {
                id = Guid.NewGuid();
            }

            var termStore = (TermStore) OfficeDevPnP.SPOnline.Core.SPOTaxonomy.GetDefaultKeywordsTermStore(ClientContext);
            var termSet = (TermSet) OfficeDevPnP.SPOnline.Core.SPOTaxonomy.GetTaxonomyItemByPath(TermSetPath, ClientContext);
            OfficeDevPnP.SPOnline.Core.SPOField.AddTaxonomyField(list, DisplayName, InternalName, Group, termStore, termSet, id, Required, AddToDefaultView, MultiValue, ClientContext);
        }

    }

}
