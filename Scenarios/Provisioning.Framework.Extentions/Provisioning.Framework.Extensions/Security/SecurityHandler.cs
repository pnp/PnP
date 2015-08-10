using System;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Extensibility;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using Schema = Provisioning.Framework.Extensions.SecurityProvisionSchema;
using Microsoft.SharePoint.Client.Utilities;
using System.Text;

namespace Provisioning.Framework.Extensions
{
    class SecurityHandler : IProvisioningExtensibilityProvider
    {
        public void ProcessRequest(ClientContext ctx, ProvisioningTemplate template, string configurationData)
        {
            var config = !string.IsNullOrEmpty(configurationData) ? XmlHelper.ReadXmlString<Schema.SecurityConfiguration>(configurationData) : null;
            if (config != null)
            {
                foreach(var level in config.PermissionLevels) 
                {
                    ProvisionPermissionLevel(ctx, level);
                }

                foreach(var group in config.Groups) 
                {
                    ProvisionGroup(ctx, group);
                }

                foreach(var scope in config.PermissionScopes) 
                {
                    ProvisionListScope(ctx, scope);
                }
            }
        }

        private void ProvisionListScope(ClientContext ctx, Schema.ListPermissionScope scope)
        {
            var web = ctx.Web;
            var targetList = web.Lists.GetByTitle(scope.Title);
            ctx.Load(targetList, l => l.RootFolder, l => l.RootFolder.ServerRelativeUrl);
            foreach (var permission in scope.Permissions)
            {
                var principal = ResolvePrincipal(ctx, permission.Principal);
                targetList.BreakRoleInheritance(scope.CopyRoleAssignments, scope.ClearSubscopes);
                AssignPermissionLevel(ctx, targetList, principal, permission.PermissionLevel);
            }

            if (scope.FolderScope != null && scope.FolderScope.Length > 0)
            {
                foreach (var folder in scope.FolderScope)
                {
                    if (folder.Permissions != null && folder.Permissions.Length > 0)
                    {
                        var folderUrl = GetListSubfolderUrl(targetList, folder.RelativeUrl);
                        var targetFolder = web.GetFolderByServerRelativeUrl(folderUrl).ListItemAllFields;
                        foreach (var permission in folder.Permissions)
                        {
                            var principal = ResolvePrincipal(ctx, permission.Principal);
                            targetFolder.BreakRoleInheritance(folder.CopyRoleAssignments, folder.ClearSubscopes);
                            AssignPermissionLevel(ctx, targetFolder, principal, permission.ToString());
                        }
                    }
                }
            }
        }

        private void ProvisionPermissionLevel(ClientContext ctx, Schema.PermissionLevel permissionLevel)
        {
            if (!PermissionLevelExist(ctx, permissionLevel.Title))
            {
                var levelPermissions = new BasePermissions();
                foreach (var permission in permissionLevel.Permissions)
                {
                    PermissionKind parsedPermission;
                    if (Enum.TryParse(permission.ToString(), out parsedPermission))
                    {
                        levelPermissions.Set(parsedPermission);
                    }
                    else
                    {
                        throw new Exception(String.Format("Unknown permission name: {0}", permission));
                    }
                }
                var creationInfo = new RoleDefinitionCreationInformation
                {
                    Name = permissionLevel.Title,
                    Description = permissionLevel.Description,
                    BasePermissions = levelPermissions
                };
                ctx.Web.RoleDefinitions.Add(creationInfo);
                ctx.ExecuteQueryRetry();
            }
        }

        private void ProvisionGroup(ClientContext ctx, Schema.Group group)
        {
            var web = ctx.Web;
            var siteGroups = web.SiteGroups;
            ctx.Load(siteGroups, sg => sg.Include(g => g.Title));
            ctx.ExecuteQueryRetry();

            Group targetGroup;
            // check if group exists
            if (siteGroups.Any(sg => sg.Title == group.Title))
            {
                targetGroup = siteGroups.First(sg => sg.Title == group.Title);
            }
            else
            {
                // create group
                var newGroupInfo = new GroupCreationInformation
                {
                    Title = group.Title,
                    Description = group.Description
                };
                targetGroup = web.SiteGroups.Add(newGroupInfo);
            }
            // assign permission level
            AssignPermissionLevel(ctx, web, targetGroup, group.PermissionLevel);
            
            // add members
            if (group.Members != null)
            {
                foreach (var user in group.Members)
                {
                    var spUser = web.EnsureUser(user);
                    ctx.Load(spUser);
                    targetGroup.Users.AddUser(spUser);
                }
                ctx.ExecuteQueryRetry();
            }
        }

        private bool PermissionLevelExist(ClientContext ctx, string permissionLevelName)
        {
            bool res;
            try
            {
                var level = ctx.Web.RoleDefinitions.GetByName(permissionLevelName);
                ctx.Load(level);
                ctx.ExecuteQueryRetry();
                res = true;
            }
            catch (ServerException ex)
            {
                if (ex.ServerErrorCode == -2146232832)
                {
                    res = false;
                }
                else
                {
                    throw;
                }
            }
            return res;
        }

        private static void AssignPermissionLevel(ClientContext ctx, SecurableObject target, Principal principal, string permissionLevel, bool reset = false)
        {
            RoleDefinition roleDefinition;
            DefaultPermissionLevels permission;
            // find OOB or custom RoleDefinition
            if (Enum.TryParse(permissionLevel, out permission))
            {
                roleDefinition = ctx.Web.RoleDefinitions.GetById((int)permission);
            }
            else
            {
                roleDefinition = ctx.Web.RoleDefinitions.GetByName(permissionLevel);
            }
            var bindingCollection = new RoleDefinitionBindingCollection(ctx) { roleDefinition };
            // cleanup existing assignments if needed
            if (reset)
            {
                var assignment = target.RoleAssignments.GetByPrincipal(principal);
                ctx.Load(assignment);
                assignment.RoleDefinitionBindings.RemoveAll();
                assignment.Update();
            }
            // assign permission level and save changes
            target.RoleAssignments.Add(principal, bindingCollection);
            ctx.ExecuteQueryRetry();
        }

        private static Principal ResolvePrincipal(ClientContext ctx, string principalName)
        {
            Principal principal = null;
            var info = Utility.ResolvePrincipal(ctx, ctx.Web, principalName, PrincipalType.User | PrincipalType.SecurityGroup | PrincipalType.SharePointGroup, PrincipalSource.All, null, false);
            ctx.ExecuteQueryRetry();
            switch (info.Value.PrincipalType)
            {
                case PrincipalType.User:
                    principal = ctx.Web.EnsureUser(info.Value.LoginName);
                    break;
                case PrincipalType.SharePointGroup:
                case PrincipalType.SecurityGroup:
                    principal = ctx.Web.SiteGroups.GetById(info.Value.PrincipalId);
                    break;
            }
            ctx.Load(principal);
            ctx.ExecuteQueryRetry();
            return principal;
        }

        private static string GetListSubfolderUrl(List list, string relativeUrl)
        {
            var sb = new StringBuilder(list.RootFolder.ServerRelativeUrl);
            sb.AppendFormat("/{0}", relativeUrl);
            return sb.ToString();
        }
    }
}
