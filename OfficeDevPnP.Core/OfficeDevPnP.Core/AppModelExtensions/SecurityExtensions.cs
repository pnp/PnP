using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Online.SharePoint.TenantAdministration;
using Microsoft.Online.SharePoint.TenantManagement;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Utilities;

namespace Microsoft.SharePoint.Client
{
    /// <summary>
    /// This manager class holds security related methods
    /// </summary>
    public static partial class SecurityExtensions
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
            web.Context.ExecuteQueryRetry();

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
                web.Context.ExecuteQueryRetry();

                //now that the user exists in the context, update to be an admin
                addedAdmin.IsSiteAdmin = true;
                addedAdmin.Update();

                if (addToOwnersGroup)
                {
                    web.AssociatedOwnerGroup.Users.AddUser(addedAdmin);
                    web.AssociatedOwnerGroup.Update();
                }
                web.Context.ExecuteQueryRetry();
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
            web.Context.ExecuteQueryRetry();

            var adminToRemove = users.FirstOrDefault(u => String.Equals(u.LoginName, admin.LoginName, StringComparison.CurrentCultureIgnoreCase));
            if (adminToRemove != null && adminToRemove.IsSiteAdmin)
            {
                adminToRemove.IsSiteAdmin = false;
                adminToRemove.Update();
                web.Context.ExecuteQueryRetry();
            }

        }


        #endregion

        #region Permissions management
        /// <summary>
        /// Add read access to the group "Everyone except external users".
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        public static User AddReaderAccess(this Web web)
        {
            return AddReaderAccessImplementation(web, BuiltInIdentity.EveryoneButExternalUsers);
        }

        /// <summary>
        /// Add read access to the group "Everyone except external users".
        /// </summary>
        /// <param name="web">Site to be processed - can be root web or sub site</param>
        /// <param name="user">Built in user to add to the visitors group</param>
        public static User AddReaderAccess(this Web web, BuiltInIdentity user)
        {
            return AddReaderAccessImplementation(web, user);
        }

        private static User AddReaderAccessImplementation(Web web, BuiltInIdentity user)
        {
            switch (user)
            {
                case BuiltInIdentity.Everyone:
                    {
                        const string userIdentity = "c:0(.s|true";
                        User spReader = web.EnsureUser(userIdentity);
                        web.Context.Load(spReader);
                        web.Context.ExecuteQueryRetry();

                        web.AssociatedVisitorGroup.Users.AddUser(spReader);
                        web.AssociatedVisitorGroup.Update();
                        web.Context.ExecuteQueryRetry();
                        return spReader;
                    }
                case BuiltInIdentity.EveryoneButExternalUsers:
                    {
                        User spReader = null;
                        try
                        {
                            // New tenant
                            string userIdentity = string.Format("c:0-.f|rolemanager|spo-grid-all-users/{0}", web.GetAuthenticationRealm());
                            spReader = web.EnsureUser(userIdentity);
                            web.Context.Load(spReader);
                            web.Context.ExecuteQueryRetry();
                        }
                        catch (ServerException)
                        {
                            // old tenant?
                            string userIdentity = string.Empty;

                            web.Context.Load(web, w => w.Language);
                            web.Context.ExecuteQueryRetry();

                            switch (web.Language)
                            {
                                case 1025: // Arabic
                                    userIdentity = "الجميع باستثناء المستخدمين الخارجيين";
                                    break;
                                case 1069: // Basque
                                    userIdentity = "Guztiak kanpoko erabiltzaileak izan ezik";
                                    break;
                                case 1026: // Bulgarian
                                    userIdentity = "Всички освен външни потребители";
                                    break;
                                case 1027: // Catalan
                                    userIdentity = "Tothom excepte els usuaris externs";
                                    break;
                                case 2052: // Chinese (Simplified)
                                    userIdentity = "除外部用户外的任何人";
                                    break;
                                case 1028: // Chinese (Traditional)
                                    userIdentity = "外部使用者以外的所有人";
                                    break;
                                case 1050: // Croatian
                                    userIdentity = "Svi osim vanjskih korisnika";
                                    break;
                                case 1029: // Czech
                                    userIdentity = "Všichni kromě externích uživatelů";
                                    break;
                                case 1030: // Danish
                                    userIdentity = "Alle undtagen eksterne brugere";
                                    break;
                                case 1043: // Dutch
                                    userIdentity = "Iedereen behalve externe gebruikers";
                                    break;
                                case 1033: // English
                                    userIdentity = "Everyone except external users";
                                    break;
                                case 1061: // Estonian
                                    userIdentity = "Kõik peale väliskasutajate";
                                    break;
                                case 1035: // Finnish
                                    userIdentity = "Kaikki paitsi ulkoiset käyttäjät";
                                    break;
                                case 1036: // French
                                    userIdentity = "Tout le monde sauf les utilisateurs externes";
                                    break;
                                case 1110: // Galician
                                    userIdentity = "Todo o mundo excepto os usuarios externos";
                                    break;
                                case 1031: // German
                                    userIdentity = "Jeder, außer externen Benutzern";
                                    break;
                                case 1032: // Greek
                                    userIdentity = "Όλοι εκτός από εξωτερικούς χρήστες";
                                    break;
                                case 1037: // Hebrew
                                    userIdentity = "כולם פרט למשתמשים חיצוניים";
                                    break;
                                case 1081: // Hindi
                                    userIdentity = "बाह्य उपयोगकर्ताओं को छोड़कर सभी";
                                    break;
                                case 1038: // Hungarian
                                    userIdentity = "Mindenki, kivéve külső felhasználók";
                                    break;
                                case 1057: // Indonesian
                                    userIdentity = "Semua orang kecuali pengguna eksternal";
                                    break;
                                case 1040: // Italian
                                    userIdentity = "Tutti tranne gli utenti esterni";
                                    break;
                                case 1041: // Japanese
                                    userIdentity = "外部ユーザー以外のすべてのユーザー";
                                    break;
                                case 1087: // Kazakh
                                    userIdentity = "Сыртқы пайдаланушылардан басқасының барлығы";
                                    break;
                                case 1042: // Korean
                                    userIdentity = "외부 사용자를 제외한 모든 사람";
                                    break;
                                case 1062: // Latvian
                                    userIdentity = "Visi, izņemot ārējos lietotājus";
                                    break;
                                case 1063: // Lithuanian
                                    userIdentity = "Visi, išskyrus išorinius vartotojus";
                                    break;
                                case 1086: // Malay
                                    userIdentity = "Semua orang kecuali pengguna luaran";
                                    break;
                                case 1044: // Norwegian (Bokmål)
                                    userIdentity = "Alle bortsett fra eksterne brukere";
                                    break;
                                case 1045: // Polish
                                    userIdentity = "Wszyscy oprócz użytkowników zewnętrznych";
                                    break;
                                case 1046: // Portuguese (Brazil)
                                    userIdentity = "Todos exceto os usuários externos";
                                    break;
                                case 2070: // Portuguese (Portugal)
                                    userIdentity = "Todos exceto os utilizadores externos";
                                    break;
                                case 1048: // Romanian
                                    userIdentity = "Toată lumea, cu excepția utilizatorilor externi";
                                    break;
                                case 1049: // Russian
                                    userIdentity = "Все, кроме внешних пользователей";
                                    break;
                                case 10266: // Serbian (Cyrillic, Serbia)
                                    userIdentity = "Сви осим спољних корисника";
                                    break;
                                case 2074:// Serbian (Latin)
                                    userIdentity = "Svi osim spoljnih korisnika";
                                    break;
                                case 1051:// Slovak
                                    userIdentity = "Všetci okrem externých používateľov";
                                    break;
                                case 1060: // Slovenian
                                    userIdentity = "Vsi razen zunanji uporabniki";
                                    break;
                                case 3082: // Spanish
                                    userIdentity = "Todos excepto los usuarios externos";
                                    break;
                                case 1053: // Swedish
                                    userIdentity = "Alla utom externa användare";
                                    break;
                                case 1054: // Thai
                                    userIdentity = "ทุกคนยกเว้นผู้ใช้ภายนอก";
                                    break;
                                case 1055: // Turkish
                                    userIdentity = "Dış kullanıcılar hariç herkes";
                                    break;
                                case 1058: // Ukranian
                                    userIdentity = "Усі, крім зовнішніх користувачів";
                                    break;
                                case 1066: // Vietnamese
                                    userIdentity = "Tất cả mọi người trừ người dùng bên ngoài";
                                    break;
                            }
                            if (!string.IsNullOrEmpty(userIdentity))
                            {
                                spReader = web.EnsureUser(userIdentity);
                                web.Context.Load(spReader);
                                web.Context.ExecuteQueryRetry();
                            }
                            else
                            {
                                throw new Exception("Language currently not supported");
                            }
                        }
                        web.AssociatedVisitorGroup.Users.AddUser(spReader);
                        web.AssociatedVisitorGroup.Update();
                        web.Context.ExecuteQueryRetry();
                        return spReader;
                    }
            }

            return null;
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
            if (siteUrl == null)
                throw new ArgumentNullException("siteUrl");

            Tenant tenant = new Tenant(web.Context);
            SiteProperties site = tenant.GetSitePropertiesByUrl(siteUrl.OriginalString, true);
            web.Context.Load(site);
            web.Context.ExecuteQueryRetry();
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
            const int pageSize = 50;
            int position = 0;

            while (true)
            {
                var results = tenant.GetExternalUsers(position, pageSize, string.Empty, SortOrder.Ascending);
                web.Context.Load(results, r => r.UserCollectionPosition, r => r.TotalUserCount, r => r.ExternalUserCollection);
                web.Context.ExecuteQueryRetry();

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
            if (siteUrl == null)
                throw new ArgumentNullException("siteUrl");

            Tenant tenantAdmin = new Tenant(web.Context);
            Office365Tenant tenant = new Office365Tenant(web.Context);
            Site site = tenantAdmin.GetSiteByUrl(siteUrl.OriginalString);
            web = site.RootWeb;

            List<ExternalUserEntity> externalUsers = new List<ExternalUserEntity>();
            const int pageSize = 50;
            int position = 0;

            while (true)
            {
                var results = tenant.GetExternalUsersForSite(siteUrl.OriginalString, position, pageSize, string.Empty, SortOrder.Ascending);
                web.Context.Load(results, r => r.UserCollectionPosition, r => r.TotalUserCount, r => r.ExternalUserCollection);
                web.Context.ExecuteQueryRetry();

                foreach (var externalUser in results.ExternalUserCollection)
                {

                    User user = web.SiteUsers.GetByEmail(externalUser.AcceptedAs);
                    web.Context.Load(user);
                    web.Context.ExecuteQueryRetry();

                    var permission = web.GetUserEffectivePermissions(user.LoginName);
                    web.Context.ExecuteQueryRetry();
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
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            int groupID = 0;

            var manageMessageGroup = web.SiteGroups.GetByName(groupName);
            web.Context.Load(manageMessageGroup);
            web.Context.ExecuteQueryRetry();
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
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

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
                web.Context.ExecuteQueryRetry();
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
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="userLoginName">Loginname of the user</param>
        public static void AddUserToGroup(this Web web, string groupName, string userLoginName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            //Ensure the user is known
            UserCreationInformation userToAdd = new UserCreationInformation();
            userToAdd.LoginName = userLoginName;
            User user = web.EnsureUser(userToAdd.LoginName);
            web.Context.Load(user);
            //web.Context.ExecuteQueryRetry();

            //Add the user to the group
            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            if (group != null)
            {
                web.AddUserToGroup(group, user);
            }
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">web to operate against</param>
        /// /// <param name="groupId">Id of the group</param>
        /// <param name="userLoginName">Login name of the user</param>
        public static void AddUserToGroup(this Web web, int groupId, string userLoginName)
        {
            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            Group group = web.SiteGroups.GetById(groupId);
            web.Context.Load(group);
            User user = web.EnsureUser(userLoginName);
            web.Context.ExecuteQueryRetry();

            if (user != null && group != null)
            {
                AddUserToGroup(web, group, user);
            }
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="group">Group object representing the group</param>
        /// <param name="user">User object representing the user</param>
        public static void AddUserToGroup(this Web web, Group group, User user)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (user == null)
                throw new ArgumentNullException("user");

            group.Users.AddUser(user);
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Adds a user to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="group">Group object representing the group</param>
        /// <param name="userLoginName">Login name of the user</param>
        public static void AddUserToGroup(this Web web, Group group, string userLoginName)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            User user = web.EnsureUser(userLoginName);
            web.Context.ExecuteQueryRetry();
            if (user != null)
            {
                group.Users.AddUser(user);
                web.Context.ExecuteQueryRetry();
            }
        }

        /// <summary>
        /// Add a permission level (e.g.Contribute, Reader,...) to a user
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="userLoginName">Loginname of the user</param>
        /// <param name="permissionLevel">Permission level to add</param>
        /// <param name="removeExistingPermissionLevels">Set to true to remove all other permission levels for that user</param>
        public static void AddPermissionLevelToUser(this Web web, string userLoginName, RoleType permissionLevel, bool removeExistingPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            User user = web.EnsureUser(userLoginName);
            web.Context.Load(user);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
            web.AddPermissionLevelImplementation(user, roleDefinition, removeExistingPermissionLevels);
        }

        /// <summary>
        /// Add a role definition (e.g.Contribute, Read, Approve) to a user
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="userLoginName">Loginname of the user</param>
        /// <param name="roleDefinitionName">Name of the role definition to add, Full Control|Design|Contribute|Read|Approve|Manage Hierarchy|Restricted Read. Use the correct name of the language of the root site you are using</param>
        /// <param name="removeExistingPermissionLevels">Set to true to remove all other permission levels for that user</param>
        public static void AddPermissionLevelToUser(this Web web, string userLoginName, string roleDefinitionName, bool removeExistingPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("roleDefinitionName");

            User user = web.EnsureUser(userLoginName);
            web.Context.Load(user);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByName(roleDefinitionName);
            web.AddPermissionLevelImplementation(user, roleDefinition, removeExistingPermissionLevels);
        }

        /// <summary>
        /// Add a permission level (e.g.Contribute, Reader,...) to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="permissionLevel">Permission level to add</param>
        /// <param name="removeExistingPermissionLevels">Set to true to remove all other permission levels for that group</param>
        public static void AddPermissionLevelToGroup(this Web web, string groupName, RoleType permissionLevel, bool removeExistingPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
            web.AddPermissionLevelImplementation(group, roleDefinition, removeExistingPermissionLevels);
        }

        /// <summary>
        /// Add a role definition (e.g.Contribute, Read, Approve) to a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="roleDefinitionName">Name of the role definition to add, Full Control|Design|Contribute|Read|Approve|Manage Hierarchy|Restricted Read. Use the correct name of the language of the root site you are using</param>
        /// <param name="removeExistingPermissionLevels">Set to true to remove all other permission levels for that group</param>
        public static void AddPermissionLevelToGroup(this Web web, string groupName, string roleDefinitionName, bool removeExistingPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("roleDefinitionName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByName(roleDefinitionName);
            web.AddPermissionLevelImplementation(group, roleDefinition, removeExistingPermissionLevels);
        }

        private static void AddPermissionLevelImplementation(this Web web, Principal principal, RoleDefinition roleDefinition, bool removeExistingPermissionLevels = false)
        {
            if (principal != null)
            {
                bool processed = false;

                RoleAssignmentCollection rac = web.RoleAssignments;
                web.Context.Load(rac);
                web.Context.ExecuteQueryRetry();

                //Find the roles assigned to the principal
                foreach (RoleAssignment ra in rac)
                {
                    // correct role assignment found
                    if (ra.PrincipalId == principal.Id)
                    {
                        // load the role definitions for this role assignment
                        RoleDefinitionBindingCollection rdc = ra.RoleDefinitionBindings;
                        web.Context.Load(rdc);
                        web.Context.Load(web.RoleDefinitions);
                        web.Context.ExecuteQueryRetry();

                        // Load the role definition to add (e.g. contribute)
                        //RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
                        if (removeExistingPermissionLevels)
                        {
                            // Remove current role definitions by removing all current role definitions
                            rdc.RemoveAll();
                        }
                        // Add the selected role definition
                        rdc.Add(roleDefinition);

                        //update                        
                        ra.ImportRoleDefinitionBindings(rdc);
                        ra.Update();
                        web.Context.ExecuteQueryRetry();

                        // Leave the for each loop
                        processed = true;
                        break;
                    }
                }

                // For a principal without role definitions set we follow a different code path
                if (!processed)
                {
                    RoleDefinitionBindingCollection rdc = new RoleDefinitionBindingCollection(web.Context);
                    rdc.Add(roleDefinition);
                    web.RoleAssignments.Add(principal, rdc);
                    web.Context.ExecuteQueryRetry();
                }
            }
        }

        /// <summary>
        /// Removes a permission level from a user
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="userLoginName">Loginname of user</param>
        /// <param name="permissionLevel">Permission level to remove. If null all permission levels are removed</param>
        /// <param name="removeAllPermissionLevels">Set to true to remove all permission level.</param>
        public static void RemovePermissionLevelFromUser(this Web web, string userLoginName, RoleType permissionLevel, bool removeAllPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            User user = web.EnsureUser(userLoginName);
            web.Context.Load(user);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
            web.RemovePermissionLevelImplementation(user, roleDefinition, removeAllPermissionLevels);
        }

        /// <summary>
        /// Removes a permission level from a user
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="userLoginName">Loginname of user</param>
        /// <param name="roleDefinitionName">Name of the role definition to add, Full Control|Design|Contribute|Read|Approve|Manage Heirarchy|Restricted Read. Use the correct name of the language of the site you are using</param>
        /// <param name="removeAllPermissionLevels">Set to true to remove all permission level.</param>
        public static void RemovePermissionLevelFromUser(this Web web, string userLoginName, string roleDefinitionName, bool removeAllPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            User user = web.EnsureUser(userLoginName);
            web.Context.Load(user);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByName(roleDefinitionName);
            web.RemovePermissionLevelImplementation(user, roleDefinition, removeAllPermissionLevels);
        }

        /// <summary>
        /// Removes a permission level from a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">name of the group</param>
        /// <param name="permissionLevel">Permission level to remove. If null all permission levels are removed</param>
        /// <param name="removeAllPermissionLevels">Set to true to remove all permission level.</param>
        public static void RemovePermissionLevelFromGroup(this Web web, string groupName, RoleType permissionLevel, bool removeAllPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByType(permissionLevel);
            web.RemovePermissionLevelImplementation(group, roleDefinition, removeAllPermissionLevels);
        }

        /// <summary>
        /// Removes a permission level from a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">name of the group</param>
        /// <param name="roleDefinitionName">Name of the role definition to add, Full Control|Design|Contribute|Read|Approve|Manage Heirarchy|Restricted Read. Use the correct name of the language of the site you are using</param>
        /// <param name="removeAllPermissionLevels">Set to true to remove all permission level.</param>
        public static void RemovePermissionLevelFromGroup(this Web web, string groupName, string roleDefinitionName, bool removeAllPermissionLevels = false)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            RoleDefinition roleDefinition = web.RoleDefinitions.GetByName(roleDefinitionName);
            web.RemovePermissionLevelImplementation(group, roleDefinition, removeAllPermissionLevels);
        }

        private static void RemovePermissionLevelImplementation(this Web web, Principal principal, RoleDefinition roleDefinition, bool removeAllPermissionLevels = false)
        {
            if (principal != null)
            {
                RoleAssignmentCollection rac = web.RoleAssignments;
                web.Context.Load(rac);
                web.Context.ExecuteQueryRetry();

                //Find the roles assigned to the principal
                foreach (RoleAssignment ra in rac)
                {
                    // correct role assignment found
                    if (ra.PrincipalId == principal.Id)
                    {
                        // load the role definitions for this role assignment
                        RoleDefinitionBindingCollection rdc = ra.RoleDefinitionBindings;
                        web.Context.Load(rdc);
                        web.Context.Load(web.RoleDefinitions);
                        web.Context.ExecuteQueryRetry();

                        if (removeAllPermissionLevels)
                        {
                            // Remove current role definitions by removing all current role definitions
                            rdc.RemoveAll();
                        }
                        else
                        {
                            // Load the role definition to remove (e.g. contribute)
                            rdc.Remove(roleDefinition);
                        }

                        //update                      
                        ra.ImportRoleDefinitionBindings(rdc);
                        ra.Update();
                        web.Context.ExecuteQueryRetry();

                        // Leave the for each loop
                        break;
                    }
                }
            }
        }

        /// <summary>
        /// Removes a user from a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="userLoginName">Loginname of the user</param>
        public static void RemoveUserFromGroup(this Web web, string groupName, string userLoginName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            if (group != null)
            {
                User user = group.Users.GetByLoginName(userLoginName);
                web.Context.Load(user);
                web.Context.ExecuteQueryRetry();
                if (!user.ServerObjectIsNull.Value)
                {
                    web.RemoveUserFromGroup(group, user);
                }
            }
        }

        /// <summary>
        /// Removes a user from a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="group">Group object to operate against</param>
        /// <param name="user">User object that needs to be removed</param>
        public static void RemoveUserFromGroup(this Web web, Group group, User user)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            if (user == null)
                throw new ArgumentNullException("user");

            group.Users.Remove(user);
            group.Update();
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Remove a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        public static void RemoveGroup(this Web web, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            var group = web.SiteGroups.GetByName(groupName);
            web.Context.Load(group);
            web.Context.ExecuteQueryRetry();
            if (group != null)
            {
                web.RemoveGroup(group);
            }
        }

        /// <summary>
        /// Remove a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="group">Group object to remove</param>
        public static void RemoveGroup(this Web web, Group group)
        {
            if (group == null)
                throw new ArgumentNullException("group");

            GroupCollection groups = web.SiteGroups;
            groups.Remove(group);
            web.Context.ExecuteQueryRetry();
        }

        /// <summary>
        /// Checks if a user is member of a group
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <param name="userLoginName">Loginname of the user</param>
        /// <returns>True if the user is in the group, false otherwise</returns>
        public static bool IsUserInGroup(this Web web, string groupName, string userLoginName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            if (string.IsNullOrEmpty(userLoginName))
                throw new ArgumentNullException("userLoginName");

            bool result = false;

            var group = web.SiteGroups.GetByName(groupName);
            var users = group.Users;
            web.Context.Load(group);
            web.Context.Load(users);
            web.Context.ExecuteQueryRetry();
            if (group != null)
            {
                result = users.Any(u => u.LoginName.Contains(userLoginName));
            }

            return result;
        }

        /// <summary>
        /// Checks if a group exists
        /// </summary>
        /// <param name="web">Web to operate against</param>
        /// <param name="groupName">Name of the group</param>
        /// <returns>True if the group exists, false otherwise</returns>
        public static bool GroupExists(this Web web, string groupName)
        {
            if (string.IsNullOrEmpty(groupName))
                throw new ArgumentNullException("groupName");

            bool result = false;

            try
            {
                var group = web.SiteGroups.GetByName(groupName);
                web.Context.Load(group);
                web.Context.ExecuteQueryRetry();
                if (group != null)
                {
                    result = true;
                }
            }
            catch (ServerException ex)
            {
                if (IsGroupCannotBeFoundException(ex))
                {
                    //eat the exception
                }
                else
                {
                    //rethrow exception
                    throw;
                }
            }

            return result;
        }

        private static bool IsGroupCannotBeFoundException(Exception ex)
        {
            if (ex is ServerException)
            {
                if (((ServerException)ex).ServerErrorCode == -2146232832 && ((ServerException)ex).ServerErrorTypeName.Equals("Microsoft.SharePoint.SPException", StringComparison.InvariantCultureIgnoreCase))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }
        #endregion

        /// <summary>
        /// Returns the authentication realm for the current web
        /// </summary>
        /// <param name="web"></param>
        /// <returns></returns>
        public static Guid GetAuthenticationRealm(this Web web)
        {

            Guid returnGuid = Guid.Empty;
            if (!web.IsPropertyAvailable("Url"))
            {
                web.Context.Load(web, w => w.Url);
                web.Context.ExecuteQueryRetry();
            }

            returnGuid = new Guid(TokenHelper.GetRealmFromTargetUrl(new Uri(web.Url)));

            return returnGuid;

        }
    }
}
