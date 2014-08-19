using OfficeDevPnP.SPOnline.CmdletHelpAttributes;
using OfficeDevPnP.SPOnline.Commands.Base;
using OfficeDevPnP.SPOnline.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using SPO = OfficeDevPnP.SPOnline.Core;

namespace OfficeDevPnP.SPOnline.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOField")]
    public class GetField : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SPOFieldIdPipeBind Identity = new SPOFieldIdPipeBind();

        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = this.SelectedWeb.GetList(List);

                Field f = null;
                FieldCollection c = null;
                if (list != null)
                {
                    if (Identity.Id != Guid.Empty)
                    {
                        f = list.Fields.GetById(Identity.Id);
                    }
                    else if (!string.IsNullOrEmpty(Identity.Name))
                    {
                        f = list.Fields.GetByInternalNameOrTitle(Identity.Name);
                    }
                    else
                    {
                        c = list.Fields;
                        ClientContext.Load(c);
                        ClientContext.ExecuteQuery();
                    }
                }
                if (f != null)
                {
                    ClientContext.Load(f);
                    ClientContext.ExecuteQuery();
                    WriteObject(f);
                }
                else if (c != null)
                {

                    WriteObject(c, true);
                }
                else
                {
                    WriteObject(null);
                }
            }
            else
            {
                ClientContext.Load(this.SelectedWeb.Fields);
                // Get a site column
                if (Identity.Id == Guid.Empty && string.IsNullOrEmpty(Identity.Name))
                {
                    // Get all columns
                    ClientContext.ExecuteQuery();
                    WriteObject(this.SelectedWeb.Fields, true);
                }
                else
                {
                    Field f = null;
                    if (Identity.Id != Guid.Empty)
                    {
                        f = this.SelectedWeb.Fields.GetById(Identity.Id);
                    }
                    else if (!string.IsNullOrEmpty(Identity.Name))
                    {
                        f = this.SelectedWeb.Fields.GetByInternalNameOrTitle(Identity.Name);
                    }
                    ClientContext.Load(f);
                    ClientContext.ExecuteQuery();
                    WriteObject(f);
                }
            }

        }
    }

}
