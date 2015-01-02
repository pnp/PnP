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
        [Parameter(Mandatory=true)]
        public GroupPipeBind Identity;

        [Parameter(Mandatory = false)]
        public AssociatedGroupTypeEnum SetAssociatedGroup = AssociatedGroupTypeEnum.None;

        [Parameter(Mandatory = false)]
        public string AddRole = string.Empty;

        [Parameter(Mandatory = false)]
        public string RemoveRole = string.Empty;

        protected override void ExecuteCmdlet()
        {
            Group group = null;
            if (Identity.Id != -1)
            {
                group = this.SelectedWeb.SiteGroups.GetById(Identity.Id);
            }
            else if (!string.IsNullOrEmpty(Identity.Name))
            {
                group = this.SelectedWeb.SiteGroups.GetByName(Identity.Name);
            }
            else if (Identity.Group != null)
            {
                group = Identity.Group;
            }

            if (SetAssociatedGroup != AssociatedGroupTypeEnum.None)
            {
                switch (SetAssociatedGroup)
                {
                    case AssociatedGroupTypeEnum.Visitors:
                        {
                            this.SelectedWeb.AssociateDefaultGroups(null, null, group);
                            break;
                        }
                    case AssociatedGroupTypeEnum.Members:
                        {
                            this.SelectedWeb.AssociateDefaultGroups(null, group, null);
                            break;
                        }
                    case AssociatedGroupTypeEnum.Owners:
                        {
                            this.SelectedWeb.AssociateDefaultGroups(group, null, null);
                            break;
                        }
                }
            }
            if(!string.IsNullOrEmpty(AddRole))
            {
                var roleDefinition = this.SelectedWeb.RoleDefinitions.GetByName(AddRole);
                var roleDefinitionBindings = new RoleDefinitionBindingCollection(ClientContext);
                roleDefinitionBindings.Add(roleDefinition);
                var roleAssignments = this.SelectedWeb.RoleAssignments;
                roleAssignments.Add(group,roleDefinitionBindings);
                ClientContext.Load(roleAssignments);
                ClientContext.ExecuteQuery();
            }
            if(!string.IsNullOrEmpty(RemoveRole))
            {
                var roleAssignment = this.SelectedWeb.RoleAssignments.GetByPrincipal(group);
                var roleDefinitionBindings = roleAssignment.RoleDefinitionBindings;
                ClientContext.Load(roleDefinitionBindings);
                ClientContext.ExecuteQuery();
                foreach(var roleDefinition in roleDefinitionBindings)
                {
                    if(roleDefinition.Name == RemoveRole)
                    {
                        roleDefinitionBindings.Remove(roleDefinition);
                        roleAssignment.Update();
                        ClientContext.ExecuteQuery();
                        break;
                    }
                }
            }
        }
    }
}
