using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using OfficeDevPnP.PowerShell.Commands.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;

namespace OfficeDevPnP.PowerShell.Commands.Principals
{
    [Cmdlet("Set", "SPOGroup")]
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

            if(!string.IsNullOrEmpty(Title))
            {
                group.Title = Title;
                group.Update();
                ClientContext.ExecuteQueryRetry();
            }
            
        }
    }
}
