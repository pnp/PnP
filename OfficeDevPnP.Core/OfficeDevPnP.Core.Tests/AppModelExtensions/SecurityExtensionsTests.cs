using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Tests;

namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass]
    public class SecurityExtensionsTests
    {
        private readonly string _testGroupName = "Group_" + Guid.NewGuid();
        private string _userLogin;

        [TestInitialize]
        public void Initialize()
        {
            _userLogin = ConfigurationManager.AppSettings["SPOUserName"];

            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.AddGroup(_testGroupName, "", true, true);
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.RemoveGroup(_testGroupName);
            }
        }

        [TestMethod]
        public void GetAdministratorsTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                Assert.IsTrue(clientContext.Web.GetAdministrators().Any(), "No administrators returned");
            }
        }

        [TestMethod]
        public void AddAdministratorsTest()
        {
            // Difficult to test on a developer (MSDN) tenant, as there is only one user allowed.
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Count admins
                int initialCount = clientContext.Web.GetAdministrators().Count;
                var userEntity = new UserEntity {LoginName = _userLogin, Email = _userLogin};
                clientContext.Web.AddAdministrators(new List<UserEntity> {userEntity}, false);

                int newCount = clientContext.Web.GetAdministrators().Count;
                Assert.IsTrue(initialCount == newCount);

                // Assumes that we're on a dev tenant, and that the existing sitecol admin is the same as the user being added.
                clientContext.Web.RemoveAdministrator(userEntity);
            }
        }

        [TestMethod]
        public void AddGroupTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Test
                Group group = clientContext.Web.AddGroup("Test Group", "Test Description", true);
                Assert.IsInstanceOfType(group, typeof (Group), "Group object returned not of correct type");
                Assert.IsTrue(group.Title == "Test Group", "Group not created with correct title");

                // Cleanup
                if (group != null)
                {
                    clientContext.Web.RemoveGroup(group);
                }
            }
        }

        [TestMethod]
        public void AddPermissionLevelToGroupTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddPermissionLevelToGroup(_testGroupName, RoleType.Contributor, false);

                //Get Group
                Group group = clientContext.Web.SiteGroups.GetByName(_testGroupName);
                clientContext.ExecuteQuery();

                //Assert
                Assert.IsTrue(CheckPermissionOnPrinciple(clientContext.Web, group, RoleType.Contributor));
            }
        }

        [TestMethod]
        public void AddPermissionLevelByRoleDefToGroupTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddPermissionLevelToGroup(_testGroupName, "Approve", false);

                //Get Group
                Group group = clientContext.Web.SiteGroups.GetByName(_testGroupName);
                clientContext.ExecuteQuery();

                //Assert 
                Assert.IsTrue(CheckPermissionOnPrinciple(clientContext.Web, group, "Approve"));
            }
        }

        [TestMethod]
        public void AddPermissionLevelToUserTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                Web web = clientContext.Web;

                var roleType = RoleType.Contributor;

                //Setup: Make sure permission does not already exist
                web.RemovePermissionLevelFromUser(_userLogin, roleType);

                //Add Permission
                web.AddPermissionLevelToUser(_userLogin, roleType);

                //Get User
                User user = web.EnsureUser(_userLogin);
                clientContext.ExecuteQuery();

                //Assert
                Assert.IsTrue(CheckPermissionOnPrinciple(web, user, roleType));

                //Teardown: Expicitly remove given permission. 
                web.RemovePermissionLevelFromUser(_userLogin, roleType);
            }
        }

        private bool CheckPermissionOnPrinciple(Web web, Principal principle, RoleType roleType)
        {
            //Get Roles for the User
            RoleDefinitionBindingCollection roleDefinitionBindingCollection =
                web.RoleAssignments.GetByPrincipal(principle).RoleDefinitionBindings;
            web.Context.Load(roleDefinitionBindingCollection);
            web.Context.ExecuteQuery();

            //Check if assigned role is found
            bool roleExists = false;
            foreach (RoleDefinition rd in roleDefinitionBindingCollection)
            {
                if (rd.RoleTypeKind == roleType)
                {
                    roleExists = true;
                }
            }

            return roleExists;
        }

        private bool CheckPermissionOnPrinciple(Web web, Principal principle, string roleDefinitionName)
        {
            //Get Roles for the User
            RoleDefinitionBindingCollection roleDefinitionBindingCollection =
                web.RoleAssignments.GetByPrincipal(principle).RoleDefinitionBindings;
            web.Context.Load(roleDefinitionBindingCollection);
            web.Context.ExecuteQuery();

            //Check if assigned role is found
            bool roleExists = false;
            foreach (RoleDefinition rd in roleDefinitionBindingCollection)
            {
                if (rd.Name == roleDefinitionName)
                {
                    roleExists = true;
                }
            }

            return roleExists;
        }

        [TestMethod]
        public void AddPermissionLevelToUserTestByRoleDef()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                Web web = clientContext.Web;

                //Setup: Make sure permission does not already exist
                web.RemovePermissionLevelFromUser(_userLogin, "Approve");

                //Add Permission
                web.AddPermissionLevelToUser(_userLogin, "Approve");

                //Get User
                User user = web.EnsureUser(_userLogin);
                clientContext.ExecuteQuery();

                //Assert
                Assert.IsTrue(CheckPermissionOnPrinciple(web, user, "Approve"));

                //Teardown: Expicitly remove given permission. 
                web.RemovePermissionLevelFromUser(_userLogin, "Approve");
            }
        }

        [TestMethod]
        public void AddReaderAccessTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                User userIdentity = null;

                // Test
                userIdentity = clientContext.Web.AddReaderAccess();

                Assert.IsNotNull(userIdentity, "No user added");
                User existingUser = clientContext.Web.AssociatedVisitorGroup.Users.GetByLoginName(userIdentity.LoginName);

                Assert.IsNotNull(existingUser, "No user returned");
                Assert.IsInstanceOfType(existingUser, typeof (User), "Object returned not of correct type");

                // Cleanup
                if (existingUser != null)
                {
                    clientContext.Web.AssociatedVisitorGroup.Users.Remove(existingUser);
                    clientContext.Web.AssociatedVisitorGroup.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        [TestMethod]
        public void AddReaderAccessTest1()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                string userIdentity = "c:0(.s|true";

                // Test
                clientContext.Web.AddReaderAccess(BuiltInIdentity.Everyone);

                User existingUser = clientContext.Web.AssociatedVisitorGroup.Users.GetByLoginName(userIdentity);
                Assert.IsNotNull(existingUser, "No user returned");
                Assert.IsInstanceOfType(existingUser, typeof (User), "Object returned not of correct type");

                // Cleanup
                if (existingUser != null)
                {
                    clientContext.Web.AssociatedVisitorGroup.Users.Remove(existingUser);
                    clientContext.Web.AssociatedVisitorGroup.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }
    }
}