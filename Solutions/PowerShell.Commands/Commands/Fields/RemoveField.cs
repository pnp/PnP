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
        public SPOFieldIdPipeBind Identity = new SPOFieldIdPipeBind();

        [Parameter(Mandatory = true, ValueFromPipeline = true, Position = 1)]
        public SPOListPipeBind List;

        [Parameter(Mandatory = false)]
        public SwitchParameter Force;

        protected override void ExecuteCmdlet()
        {
            var list = this.SelectedWeb.GetList(List);

            Field f = null;
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
                if (f != null)
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
