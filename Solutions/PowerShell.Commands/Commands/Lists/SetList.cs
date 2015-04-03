using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Lists
{
    [Cmdlet(VerbsCommon.Set, "SPOList")]
    [CmdletHelp("Updates list settings", Category = "Lists")]
    public class SetList : SPOWebCmdlet
    {
        [Parameter(Mandatory=true)]
        public ListPipeBind Identity;

        [Parameter(Mandatory = false)]
        public SwitchParameter BreakRoleInheritance;

        [Parameter(Mandatory = false)]
        public SwitchParameter CopyRoleAssignments;

        [Parameter(Mandatory = false)]
        public SwitchParameter ClearSubscopes;

        [Parameter(Mandatory = false)]
        public string Title = string.Empty;

        protected override void ExecuteCmdlet()
        {
            var list = SelectedWeb.GetList(Identity);

            if(list != null)
            {
                if(BreakRoleInheritance)
                {
                    list.BreakRoleInheritance(CopyRoleAssignments, ClearSubscopes);
                    list.Update();
                    ClientContext.ExecuteQueryRetry();
                }

                if (!string.IsNullOrEmpty(Title))
                {
                    list.Title = Title;
                    list.Update();
                    ClientContext.ExecuteQueryRetry();
                }
            }
        }
    }
}
