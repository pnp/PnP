using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base;
using System.Management.Automation;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Get, "SPOGroup",DefaultParameterSetName="All")]
    [CmdletHelp("Returns a specific group or all groups.")]
    [CmdletExample(Code = @"
PS:> Get-SPOGroup
", SortOrder = 1)]
    [CmdletExample(Code = @"
PS:> Get-SPOGroup -Name 'Site Members'
", SortOrder = 2)]
    public class GetGroup : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, ParameterSetName = "ByName", HelpMessage = "Get a specific group by name")]
        public string Name = string.Empty;

        [Parameter(Mandatory = false, ParameterSetName = "Members", HelpMessage = "Retrieve the associated member group")]
        public SwitchParameter AssociatedMemberGroup;

        [Parameter(Mandatory = false, ParameterSetName = "Visitors", HelpMessage = "Retrieve the associated visitor group")]
        public SwitchParameter AssociatedVisitorGroup;

        [Parameter(Mandatory = false, ParameterSetName = "Owners", HelpMessage = "Retrieve the associated owner group")]
        public SwitchParameter AssociatedOwnerGroup;

        protected override void ExecuteCmdlet()
        {
            if (ParameterSetName == "ByName")
            {
                var group = this.SelectedWeb.SiteGroups.GetByName(Name);

                ClientContext.Load(group);
                ClientContext.Load(group.Users);

                ClientContext.ExecuteQuery();

                WriteObject(group);
            }
            else if (ParameterSetName == "Members")
            {
                ClientContext.Load(this.SelectedWeb.AssociatedMemberGroup);
                ClientContext.Load(this.SelectedWeb.AssociatedMemberGroup.Users);
                ClientContext.ExecuteQuery();
                WriteObject(this.SelectedWeb.AssociatedMemberGroup);
            }
            else if (ParameterSetName == "Visitors")
            {
                ClientContext.Load(this.SelectedWeb.AssociatedVisitorGroup);
                ClientContext.Load(this.SelectedWeb.AssociatedVisitorGroup.Users);
                ClientContext.ExecuteQuery();
                WriteObject(this.SelectedWeb.AssociatedVisitorGroup);
            }
            else if (ParameterSetName == "Owners")
            {
                ClientContext.Load(this.SelectedWeb.AssociatedOwnerGroup);
                ClientContext.Load(this.SelectedWeb.AssociatedOwnerGroup.Users);
                ClientContext.ExecuteQuery();
                WriteObject(this.SelectedWeb.AssociatedOwnerGroup);
            }
            else if (ParameterSetName == "All")
            {
                var groups = ClientContext.LoadQuery(this.SelectedWeb.SiteGroups.IncludeWithDefaultProperties(g => g.Users));
                ClientContext.ExecuteQuery();
                WriteObject(groups);
            }

        }
    }



}
