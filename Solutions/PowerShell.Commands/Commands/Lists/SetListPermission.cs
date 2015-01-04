using Microsoft.SharePoint.Client;
using OfficeDevPnP.PowerShell.Commands.Base.PipeBinds;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.PowerShell.Commands.Lists
{
    [Cmdlet(VerbsCommon.Set, "SPOListPermission")]
    public class SetListPermission : SPOWebCmdlet
    {
        [Parameter(Mandatory = true, ParameterSetName = ParameterAttribute.AllParameterSets)]
        public ListPipeBind Identity;

        [Parameter(Mandatory = true, ParameterSetName = "Group")]
        public GroupPipeBind Group;

        [Parameter(Mandatory = true, ParameterSetName = "User")]
        public string User;

        [Parameter(Mandatory = false)]
        public string AddRole = string.Empty;

        [Parameter(Mandatory = false)]
        public string RemoveRole = string.Empty;

        protected override void ExecuteCmdlet()
        {
            var list = this.SelectedWeb.GetList(Identity);

            if (list != null)
            {
                Principal principal = null;
                if (ParameterSetName == "Group")
                {
                    if (Group.Id != -1)
                    {
                        principal = this.SelectedWeb.SiteGroups.GetById(Group.Id);
                    }
                    else if (!string.IsNullOrEmpty(Group.Name))
                    {
                        principal = this.SelectedWeb.SiteGroups.GetByName(Group.Name);
                    }
                    else if (Group.Group != null)
                    {
                        principal = Group.Group;
                    }
                }
                else
                {
                    principal = this.SelectedWeb.EnsureUser(User);
                }
                if (principal != null)
                {
                    if (!string.IsNullOrEmpty(AddRole))
                    {
                        var roleDefinition = this.SelectedWeb.RoleDefinitions.GetByName(AddRole);
                        var roleDefinitionBindings = new RoleDefinitionBindingCollection(ClientContext);
                        roleDefinitionBindings.Add(roleDefinition);
                        var roleAssignments = list.RoleAssignments;
                        roleAssignments.Add(principal, roleDefinitionBindings);
                        ClientContext.Load(roleAssignments);
                        ClientContext.ExecuteQuery();
                    }
                    if (!string.IsNullOrEmpty(RemoveRole))
                    {
                        var roleAssignment = list.RoleAssignments.GetByPrincipal(principal);
                        var roleDefinitionBindings = roleAssignment.RoleDefinitionBindings;
                        ClientContext.Load(roleDefinitionBindings);
                        ClientContext.ExecuteQuery();
                        foreach (var roleDefinition in roleDefinitionBindings)
                        {
                            if (roleDefinition.Name == RemoveRole)
                            {
                                roleDefinitionBindings.Remove(roleDefinition);
                                roleAssignment.Update();
                                ClientContext.ExecuteQuery();
                                break;
                            }
                        }
                    }
                }
                else
                {
                    WriteError(new ErrorRecord(new Exception("Principal not found"), "1", ErrorCategory.ObjectNotFound, null));
                }
            }
        }
    }
}
