using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet(VerbsCommon.Get, "SPOGroup",DefaultParameterSetName="All")]
    [CmdletHelp("Returns a specific group or all groups.", Category = "User and group management")]
    [CmdletExample(
        Code = @"PS:> Get-SPOGroup", 
        Remarks = "Returns all groups",
        SortOrder = 1)]
    [CmdletExample(
        Code = @"PS:> Get-SPOGroup -Identity 'My Site Users'", 
        Remarks = "This will return the group called 'My Site Users' if available",
        SortOrder = 2)]
    [CmdletExample(
        Code = @"PS:> Get-SPOGroup -AssociatedMemberGroup",
        Remarks = "This will return the current members group for the site",
        SortOrder = 3)]
    public class GetGroup : SPOWebCmdlet
    {
        [Parameter(Mandatory = false, Position = 0, ValueFromPipeline = true, ParameterSetName = "ByName", HelpMessage = "Get a specific group by name")]
        [Alias("Name")]
        public GroupPipeBind Identity = new GroupPipeBind();

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
                Group group = Identity.GetGroup(SelectedWeb);
                WriteObject(group);
            }
            else if (ParameterSetName == "Members")
            {
                ClientContext.Load(SelectedWeb.AssociatedMemberGroup);
                ClientContext.Load(SelectedWeb.AssociatedMemberGroup.Users);
                ClientContext.ExecuteQueryRetry();
                WriteObject(SelectedWeb.AssociatedMemberGroup);
            }
            else if (ParameterSetName == "Visitors")
            {
                ClientContext.Load(SelectedWeb.AssociatedVisitorGroup);
                ClientContext.Load(SelectedWeb.AssociatedVisitorGroup.Users);
                ClientContext.ExecuteQueryRetry();
                WriteObject(SelectedWeb.AssociatedVisitorGroup);
            }
            else if (ParameterSetName == "Owners")
            {
                ClientContext.Load(SelectedWeb.AssociatedOwnerGroup);
                ClientContext.Load(SelectedWeb.AssociatedOwnerGroup.Users);
                ClientContext.ExecuteQueryRetry();
                WriteObject(SelectedWeb.AssociatedOwnerGroup);
            }
            else if (ParameterSetName == "All")
            {
                var groups = ClientContext.LoadQuery(SelectedWeb.SiteGroups.IncludeWithDefaultProperties(g => g.Users));
                ClientContext.ExecuteQueryRetry();
                WriteObject(groups,true);
            }

        }
    }



}
