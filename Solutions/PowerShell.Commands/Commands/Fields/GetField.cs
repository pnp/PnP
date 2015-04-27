using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOField")]
    [CmdletHelp("Returns a field from a list or site", Category = "Fields")]
    public class GetField : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ValueFromPipeline = true)]
        public ListPipeBind List;

        [Parameter(Mandatory = false, Position=0, ValueFromPipeline=true)]
        public FieldPipeBind Identity = new FieldPipeBind();

        protected override void ExecuteCmdlet()
        {
            if (List != null)
            {
                var list = SelectedWeb.GetList(List);

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
                        ClientContext.ExecuteQueryRetry();
                    }
                }
                if (f != null)
                {
                    ClientContext.Load(f);
                    ClientContext.ExecuteQueryRetry();
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

                // Get a site column
                if (Identity.Id == Guid.Empty && string.IsNullOrEmpty(Identity.Name))
                {
                    // Get all columns
                    ClientContext.Load(SelectedWeb.Fields);
                    ClientContext.ExecuteQueryRetry();
                    WriteObject(SelectedWeb.Fields, true);
                }
                else
                {
                    Field f = null;
                    if (Identity.Id != Guid.Empty)
                    {
                        f = SelectedWeb.Fields.GetById(Identity.Id);
                    }
                    else if (!string.IsNullOrEmpty(Identity.Name))
                    {
                        f = SelectedWeb.Fields.GetByInternalNameOrTitle(Identity.Name);
                    }
                    ClientContext.Load(f);
                    ClientContext.ExecuteQueryRetry();
                    WriteObject(f);
                }
            }

        }
    }

}
