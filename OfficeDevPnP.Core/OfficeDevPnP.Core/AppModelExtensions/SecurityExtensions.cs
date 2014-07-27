using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This manager class holds security related methods
    /// </summary>
    public static class SecurityExtensions
    {

        #region Site collection administrator management
        /// <summary>
        /// Get a list of site collection administrators
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <returns>List of <see cref="OfficeDevPnP.Core.Entities.UserEntity"/> objects</returns>
        public static List<UserEntity> GetAdministrators(this Web web)
        {
            var users = web.SiteUsers;
            web.Context.Load(users);
            web.Context.ExecuteQuery();

            List<UserEntity> admins = new List<UserEntity>();

            foreach (var u in users)
            {
                if (u.IsSiteAdmin)
                {
                    admins.Add(new UserEntity()
                    {
                        Title = u.Title,
                        LoginName = u.LoginName,
                        Email = u.Email,
                    });
                }
            }

            return admins;
        }

        /// <summary>
        /// Add a site collection administrator to a site collection
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="adminLogins">Array of admins loginnames to add</param>
        /// <param name="addToOwnersGroup">Optionally the added admins can also be added to the Site owners group</param>
        public static void AddAdministrators(this Web web, List<UserEntity> adminLogins, bool addToOwnersGroup = false)
        {
            var users = web.SiteUsers;
            web.Context.Load(users);

            foreach (var admin in adminLogins)
            {
                UserCreationInformation newAdmin = new UserCreationInformation();

                newAdmin.LoginName = admin.LoginName;
                //User addedAdmin = users.Add(newAdmin);
                User addedAdmin = web.EnsureUser(newAdmin.LoginName);
                web.Context.Load(addedAdmin);
                web.Context.ExecuteQuery();

                //now that the user exists in the context, update to be an admin
                addedAdmin.IsSiteAdmin = true;
                addedAdmin.Update();

                if (addToOwnersGroup)
                {
                    web.AssociatedOwnerGroup.Users.AddUser(addedAdmin);
                    web.AssociatedOwnerGroup.Update();
                }
                web.Context.ExecuteQuery();
            }            
        }

        /// <summary>
        /// Removes an administrators from the site collection
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="admin"><see cref="OfficeDevPnP.Core.Entities.UserEntity"/> that describes the admin to be removed</param>
        public static void RemoveAdministrator(this Web web, UserEntity admin)
        {
            var users = web.SiteUsers;
            web.Context.Load(users);
            web.Context.ExecuteQuery();

            var adminToRemove = users.Where(u => u.LoginName == admin.LoginName).FirstOrDefault();
            if (adminToRemove != null && adminToRemove.IsSiteAdmin)
            {
                adminToRemove.IsSiteAdmin = false;
                adminToRemove.Update();
                web.Context.ExecuteQuery();
            }
            
        }

        /// <summary>
        /// Adds additional administrators to a site collection using the Tenant administration csom. See AddAdministrators for a method
        /// that does not have a dependency on the Tenant administration csom.
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="adminLogins">Array of logins for the additional admins</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        public static void AddAdministratorsTenant(this Web web, String[] adminLogins, Uri siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);

            foreach (var admin in adminLogins)
            {
                tenant.SetSiteAdmin(siteUrl.ToString(), admin, true);
                var spAdmin = web.EnsureUser(admin);
                web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                web.AssociatedOwnerGroup.Update();
                web.Context.ExecuteQuery();
            }
        }

        /// <summary>
        /// Add a site collection administrator to a site collection
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="adminLogins">Array of admins loginnames to add</param>
        /// <param name="siteUrl">Url of the site to operate on</param>
        /// <param name="addToOwnersGroup">Optionally the added admins can also be added to the Site owners group</param>
        public static void AddAdministratorsTenant(this Web web, List<UserEntity> adminLogins, Uri siteUrl, bool addToOwnersGroup = false)
        {
            Tenant tenant = new Tenant(web.Context);

            foreach (UserEntity admin in adminLogins)
            {
                tenant.SetSiteAdmin(siteUrl.ToString(), admin.LoginName, true);
                var spAdmin = web.EnsureUser(admin.LoginName);
                if (addToOwnersGroup)
                {
                    web.AssociatedOwnerGroup.Users.AddUser(spAdmin);
                    web.AssociatedOwnerGroup.Update();
                }
                web.Context.ExecuteQuery();
            }
        }

        #endregion

        #region Permissions management
        /// <summary>
        /// Add read access to the group "Everyone except external users"
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        public static void AddReaderAccess(this Web web)
        {
            var spReader = web.EnsureUser("Everyone except external users");
            web.AssociatedVisitorGroup.Users.AddUser(spReader);
            web.AssociatedVisitorGroup.Update();
            web.Context.ExecuteQuery();
        }
        #endregion

        #region External sharing management
        /// <summary>
        /// Get the external sharing settings for the provided site. Only works in Office 365 Multi-Tenant
        /// </summary>
        /// <param name="web">Tenant administration web</param>
        /// <param name="siteUrl">Site to get the sharing capabilities from</param>
        /// <returns>Sharing capabilities of the site collection</returns>
        public static string GetSharingCapabilitiesTenant(this Web web, Uri siteUrl)
        {
            Tenant tenant = new Tenant(web.Context);
            SiteProperties site = tenant.GetSitePropertiesByUrl(siteUrl.OriginalString, true);
            web.Context.Load(site);
            web.Context.ExecuteQuery();
            return site.SharingCapability.ToString();            
        }

        /// <summary>
        /// Returns a list all external users in your tenant
        /// </summary>
        /// <param name="web">Tenant administration web</param>
        /// <returns>A list of <see cref="OfficeDevPnP.Core.Entities.ExternalUserEntity"/> objects</returns>
        public static List<ExternalUserEntity> GetExternalUsersTenant(this Web web)
        {
            Tenant tenantAdmin = new Tenant(web.Context);
            Office365Tenant tenant = new Office365Tenant(web.Context);

            List<ExternalUserEntity> externalUsers = new List<ExternalUserEntity>();
            int pageSize = 50;
            int position = 0;
            GetExternalUsersResults results = null;

            while (true)
            {
                results = tenant.GetExternalUsers(position, pageSize, string.Empty, SortOrder.Ascending);
                web.Context.Load(results, r => r.UserCollectionPosition, r => r.TotalUserCount, r => r.ExternalUserCollection);
                web.Context.ExecuteQuery();

                foreach (var externalUser in results.ExternalUserCollection)
                {
                    externalUsers.Add(new ExternalUserEntity()
                    {
                        DisplayName = externalUser.DisplayName,
                        AcceptedAs = externalUser.AcceptedAs,
                        InvitedAs = externalUser.InvitedAs,
                        InvitedBy = externalUser.InvitedBy,
                        UniqueId = externalUser.UniqueId,
                        WhenCreated = externalUser.WhenCreated
                    });
                }

                position = results.UserCollectionPosition;

                if (position == -1 || position == results.TotalUserCount)
                {
                    break;
                }
            }

            return externalUsers;
        }


        /// <summary>
        /// Returns a list all external users for a given site that have at least the viewpages permission
        /// </summary>
        /// <param name="web">Tenant administration web</param>
        /// <param name="siteUrl">Url of the site fetch the external users for</param>
        /// <returns>A list of <see cref="OfficeDevPnP.Core.Entities.ExternalUserEntity"/> objects</returns>
        public static List<ExternalUserEntity> GetExternalUsersForSiteTenant(this Web web, Uri siteUrl)
        {
            Tenant tenantAdmin = new Tenant(web.Context);
            Office365Tenant tenant = new Office365Tenant(web.Context);
            Site site = tenantAdmin.GetSiteByUrl(siteUrl.OriginalString);
            web = site.RootWeb;

            List<ExternalUserEntity> externalUsers = new List<ExternalUserEntity>();
            int pageSize = 50;
            int position = 0;
            GetExternalUsersResults results = null;

            while (true)
            {
                results = tenant.GetExternalUsersForSite(siteUrl.OriginalString, position, pageSize, string.Empty, SortOrder.Ascending);
                web.Context.Load(results, r => r.UserCollectionPosition, r => r.TotalUserCount, r => r.ExternalUserCollection);
                web.Context.ExecuteQuery();

                foreach (var externalUser in results.ExternalUserCollection)
                {

                    User user = web.SiteUsers.GetByEmail(externalUser.AcceptedAs);
                    web.Context.Load(user);
                    web.Context.ExecuteQuery();

                    var permission = web.GetUserEffectivePermissions(user.LoginName);
                    web.Context.ExecuteQuery();
                    var doesUserHavePermission = permission.Value.Has(PermissionKind.ViewPages);
                    if (doesUserHavePermission)
                    {
                        externalUsers.Add(new ExternalUserEntity()
                        {
                            DisplayName = externalUser.DisplayName,
                            AcceptedAs = externalUser.AcceptedAs,
                            InvitedAs = externalUser.InvitedAs,
                            InvitedBy = externalUser.InvitedBy,
                            UniqueId = externalUser.UniqueId,
                            WhenCreated = externalUser.WhenCreated
                        });                        
                    }

                }

                position = results.UserCollectionPosition;

                if (position == -1 || position == results.TotalUserCount)
                {
                    break;
                }
            }

            return externalUsers;
        }

        #endregion

        #region Group management
        /// <summary>
        /// Returns the integer ID for a given group name
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="groupName">SharePoint group name</param>
        /// <returns>Integer group ID</returns>
        public static int GetGroupID(this Web web, string groupName)
        {
            return web.GetGroupID(null, groupName);
        }

        /// <summary>
        /// Returns the integer ID for a given group name
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="siteUrl">Site to operate on</param>
        /// <param name="groupName">SharePoint group name</param>
        /// <returns>Integer group ID</returns>
        public static int GetGroupID(this Web web, Uri siteUrl, string groupName)
        {
            int groupID = 0;

            var manageMessageGroup = web.SiteGroups.GetByName(groupName);
            web.Context.Load(manageMessageGroup);
            web.Context.ExecuteQuery();
            if (manageMessageGroup != null)
            {
                groupID = manageMessageGroup.Id;
            }

            return groupID;
        }

        /// <summary>
        /// Adds a group
        /// </summary>
        /// <param name="web">Site to add the group to</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="groupDescription">Description of the group</param>
        /// <param name="groupIsOwner">Sets the created group as group owner if true</param>
        /// <param name="updateAndExecuteQuery">Set to false to postpone the executequery call</param>
        /// <returns>The created group</returns>
        public static Group AddGroup(this Web web, string groupName, string groupDescription, bool groupIsOwner, bool updateAndExecuteQuery = true)
        {
            GroupCreationInformation groupCreationInformation = new GroupCreationInformation();
            groupCreationInformation.Title = groupName;
            groupCreationInformation.Description = groupDescription;            
            Group group = web.SiteGroups.Add(groupCreationInformation);
            if (groupIsOwner)
            {
                group.Owner = group;
            }

            group.OnlyAllowMembersViewMembership = false;
            group.Update();
            
            if (updateAndExecuteQuery)
            {
                web.Context.ExecuteQuery();
            }

            return group;
        }

        /// <summary>
        /// Associate the provided groups as default owners, members or visitors groups. If a group is null then the 
        /// association is not done
        /// </summary>
        /// <param name="web">Site to operate on</param>
        /// <param name="owners">Owners group</param>
        /// <param name="members">Members group</param>
        /// <param name="visitors">Visitors group</param>
        public static void AssociateDefaultGroups(this Web web, Group owners, Group members, Group visitors)
        {
            if (owners != null)
            {
                web.AssociatedOwnerGroup = owners;
                web.AssociatedOwnerGroup.Update();
            }
            if (members != null)
            {
                web.AssociatedMemberGroup = members;
                web.AssociatedMemberGroup.Update();
            }
            if (visitors != null)
            {
                web.AssociatedVisitorGroup = visitors;
                web.AssociatedVisitorGroup.Update();
            }

            web.Update();
            web.Context.ExecuteQuery();
        }
        #endregion
    }
}
