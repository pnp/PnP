using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;

namespace OfficeDevPnP.PowerShell.Commands.Lists
{
    [Cmdlet(VerbsCommon.Set, "SPOList")]
    [CmdletHelp("Updates list settings", Category = "Lists")]
    [CmdletExample(
        Code = @"Set-SPOList -Identity ""Demo List"" -EnableContentTypes $true", 
        Remarks = "Switches the Enable Content Type switch on the list",
        SortOrder = 1)]
    public class SetList : SPOWebCmdlet
    {
        [Parameter(Mandatory=true)]
        public ListPipeBind Identity;

        [Parameter(Mandatory = false, HelpMessage = "Set to $true to enable content types, set to $false to disable content types")]
        public bool EnableContentTypes;

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
            var list = Identity.GetList(SelectedWeb);

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

                if (list.ContentTypesEnabled != EnableContentTypes)
                {
                    list.ContentTypesEnabled = EnableContentTypes;
                    list.Update();
                    ClientContext.ExecuteQueryRetry();
                }
            }
        }
    }
}
