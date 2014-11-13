using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using Microsoft.SharePoint.Client;
using System;
using System.Management.Automation;

namespace OfficeDevPnP.PowerShell.Commands
{
    [Cmdlet(VerbsCommon.Get, "SPOField")]
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

                // Get a site column
                if (Identity.Id == Guid.Empty && string.IsNullOrEmpty(Identity.Name))
                {
                    // Get all columns
                    ClientContext.Load(this.SelectedWeb.Fields);
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
                    try
                    {
                        ClientContext.Load(f);
                        ClientContext.ExecuteQuery();
                        WriteObject(f);
                    }
                    catch (ServerException ex)
                    {
                        // Check if the error code indicates that the field does not exists, return null instead of throwing the exception
                        if (ex.ServerErrorCode == -2147024809)
                            WriteObject(null);
                        else
                            throw ex;
                    }
                }
            }

        }
    }

}
