using System.Linq;
using System.Management.Automation;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.CmdletHelpAttributes;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet("Set", "SPOGroup")]
    [CmdletHelp("Updates a group", Category = "User and group management")]
    public class SetGroup : SPOWebCmdlet
    {
        [Parameter(Mandatory = true)]
        public GroupPipeBind Identity = new GroupPipeBind();

        [Parameter(Mandatory = false)]
        public AssociatedGroupType SetAssociatedGroup = AssociatedGroupType.None;

        [Parameter(Mandatory = false)]
        public string AddRole = string.Empty;

        [Parameter(Mandatory = false)]
        public string RemoveRole = string.Empty;

        [Parameter(Mandatory = false)]
        public string Title = string.Empty;

        [Parameter(Mandatory = false)]
        public string Owner;

        [Parameter(Mandatory = false)]
        public string Description;

        [Parameter(Mandatory = false)]
        public bool AllowRequestToJoinLeave;

        [Parameter(Mandatory = false)]
        public bool AutoAcceptRequestToJoinLeave;

        [Parameter(Mandatory = false)]
        public bool AllowMembersEditMembership;

        [Parameter(Mandatory = false)]
        public bool OnlyAllowMembersViewMembership;

        [Parameter(Mandatory = false)]
        public string RequestToJoinEmail;

        protected override void ExecuteCmdlet()
        {
            Group group = null;
            if (Identity.Id != -1)
            {
                group = SelectedWeb.SiteGroups.GetById(Identity.Id);
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                group = SelectedWeb.SiteGroups.GetByName(Identity.Name);
            }
            else if (Identity.Group != null)
            {
                group = Identity.Group;
            }

            ClientContext.Load(group, 
                g => g.AllowMembersEditMembership, 
                g => g.AllowRequestToJoinLeave, 
                g => g.AutoAcceptRequestToJoinLeave,
                g => g.OnlyAllowMembersViewMembership,
                g => g.RequestToJoinLeaveEmailSetting);
            ClientContext.ExecuteQueryRetry();
            
            if (SetAssociatedGroup != AssociatedGroupType.None)
            {
                switch (SetAssociatedGroup)
                {
                    case AssociatedGroupType.Visitors:
                        {
                            SelectedWeb.AssociateDefaultGroups(null, null, group);
                            break;
                        }
                    case AssociatedGroupType.Members:
                        {
                            SelectedWeb.AssociateDefaultGroups(null, group, null);
                            break;
                        }
                    case AssociatedGroupType.Owners:
                        {
                            SelectedWeb.AssociateDefaultGroups(group, null, null);
                            break;
                        }
                }
            }
            if(!string.IsNullOrEmpty(AddRole))
            {
                var roleDefinition = SelectedWeb.RoleDefinitions.GetByName(AddRole);
                var roleDefinitionBindings = new RoleDefinitionBindingCollection(ClientContext);
                roleDefinitionBindings.Add(roleDefinition);
                var roleAssignments = SelectedWeb.RoleAssignments;
                roleAssignments.Add(group,roleDefinitionBindings);
                ClientContext.Load(roleAssignments);
                ClientContext.ExecuteQueryRetry();
            }
            if(!string.IsNullOrEmpty(RemoveRole))
            {
                var roleAssignment = SelectedWeb.RoleAssignments.GetByPrincipal(group);
                var roleDefinitionBindings = roleAssignment.RoleDefinitionBindings;
                ClientContext.Load(roleDefinitionBindings);
                ClientContext.ExecuteQueryRetry();
                foreach (var roleDefinition in roleDefinitionBindings.Where(roleDefinition => roleDefinition.Name == RemoveRole))
                {
                    roleDefinitionBindings.Remove(roleDefinition);
                    roleAssignment.Update();
                    ClientContext.ExecuteQueryRetry();
                    break;
                }
            }

            var dirty = false;
            if (!string.IsNullOrEmpty(Title))
            {
                group.Title = Title;
                dirty = true;
            }
            if (!string.IsNullOrEmpty(Description))
            {
                group.Description = Description;
                dirty = true;
            }
            if (AllowRequestToJoinLeave != group.AllowRequestToJoinLeave)
            {
                group.AllowRequestToJoinLeave = AllowRequestToJoinLeave;
                dirty = true;
            } 

            if (AutoAcceptRequestToJoinLeave != group.AutoAcceptRequestToJoinLeave)
            {
                group.AutoAcceptRequestToJoinLeave = AutoAcceptRequestToJoinLeave;
                dirty = true;
            }
            if (AllowMembersEditMembership != group.AllowMembersEditMembership)
            {
                group.AllowMembersEditMembership = AllowMembersEditMembership;
                dirty = true;
            }
            if (OnlyAllowMembersViewMembership != group.OnlyAllowMembersViewMembership)
            {
                group.OnlyAllowMembersViewMembership = OnlyAllowMembersViewMembership;
                dirty = true;
            }
            if (RequestToJoinEmail != group.RequestToJoinLeaveEmailSetting)
            {
                group.RequestToJoinLeaveEmailSetting = RequestToJoinEmail;
                dirty = true;
            }
            if(dirty)
            {
                group.Update();
                ClientContext.ExecuteQueryRetry();
            }


            if (!string.IsNullOrEmpty(Owner))
            {
                Principal groupOwner;

                try
                {
                    groupOwner = SelectedWeb.EnsureUser(Owner);
                    group.Owner = groupOwner;
                    group.Update();
                    ClientContext.ExecuteQueryRetry();
                }
                catch
                {
                    groupOwner = SelectedWeb.SiteGroups.GetByName(Owner);
                    group.Owner = groupOwner;
                    group.Update();
                    ClientContext.ExecuteQueryRetry();
                }
            }
            
        }
    }
}
