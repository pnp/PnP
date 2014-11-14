using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Remove, "SPOField")]
    public class RemoveField : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 0)]
        public FieldPipeBind Identity = new FieldPipeBind();

        [Parameter(Mandatory = false, ValueFromPipeline = true, Position = 1)]
        public ListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = this.SelectedWeb.GetList(List);

                Field f = Identity.Field;
                if (list != null)
                {
                    if (f == null)
                    {
                        if (Identity.Id != Guid.Empty)
                        {
                            f = list.Fields.GetById(Identity.Id);
                        }
                        else if (!string.IsNullOrEmpty(Identity.Name))
                        {
                            f = list.Fields.GetByInternalNameOrTitle(Identity.Name);
                        }
                    }
                    ClientContext.Load(f);
                    ClientContext.ExecuteQuery();
                    if (f != null && f.IsPropertyAvailable("InternalName"))
                    {
                        if (Force || ShouldContinue(string.Format(Properties.Resources.DeleteField0, f.InternalName), Properties.Resources.Confirm))
                        {
                            f.DeleteObject();
                            ClientContext.ExecuteQuery();
                        }
                    }
                }
            } 
            else
            {
                Field f = Identity.Field;

                if (f == null)
                {
                    if (Identity.Id != Guid.Empty)
                    {
                        f = this.SelectedWeb.Fields.GetById(Identity.Id);
                    }
                    else if (!string.IsNullOrEmpty(Identity.Name))
                    {
                        f = this.SelectedWeb.Fields.GetByInternalNameOrTitle(Identity.Name);
                    }
                }
                ClientContext.Load(f);
                ClientContext.ExecuteQuery();

                if (f != null && f.IsPropertyAvailable("InternalName"))
                {
                    if (Force || ShouldContinue(string.Format(Properties.Resources.DeleteField0, f.InternalName), Properties.Resources.Confirm))
                    {
                        f.DeleteObject();
                        ClientContext.ExecuteQuery();
                    }
                }
            }
        }
    }

}
