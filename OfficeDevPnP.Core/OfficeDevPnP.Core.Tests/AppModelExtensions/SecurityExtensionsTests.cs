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

        #region Test initialize and cleanup
        [TestInitialize]
        public void Initialize()
        {

#if !CLIENTSDKV15
            _userLogin = ConfigurationManager.AppSettings["SPOUserName"];
            if (TestCommon.AppOnlyTesting())
            {
                using (var clientContext = TestCommon.CreateClientContext())
                {
                    List<UserEntity> admins = clientContext.Web.GetAdministrators();
                    _userLogin = admins[0].LoginName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[2];
                }
            }
#else
            _userLogin = String.Format(@"{0}\{1}", ConfigurationManager.AppSettings["OnPremDomain"], ConfigurationManager.AppSettings["OnPremUserName"]);            
            if (TestCommon.AppOnlyTesting())
            {
                using (var clientContext = TestCommon.CreateClientContext())
                {
                    List<UserEntity> admins = clientContext.Web.GetAdministrators();
                    _userLogin = admins[0].LoginName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries)[1];
                }
            }
#endif

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
        #endregion

        #region Administrator tests
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
                #if !CLIENTSDKV15
                var userEntity = new UserEntity {LoginName = _userLogin, Email = _userLogin};
                #else
                var userEntity = new UserEntity { LoginName = _userLogin };
                #endif
                clientContext.Web.AddAdministrators(new List<UserEntity> {userEntity}, false);

                List<UserEntity> admins = clientContext.Web.GetAdministrators();
                bool found = false;
                foreach(var admin in admins) 
                {                    
                    string adminLoginName = admin.LoginName;
                    String[] parts = adminLoginName.Split(new string[] { "|" }, StringSplitOptions.RemoveEmptyEntries);

                    if (parts.Length > 1)
                    {
                        adminLoginName = parts[2];
                    }
                    
                    if (adminLoginName.Equals(_userLogin, StringComparison.InvariantCultureIgnoreCase))
                    {
                        found = true;
                        break;
                    }
                }
                Assert.IsTrue(found);

                // Assumes that we're on a dev tenant, and that the existing sitecol admin is the same as the user being added.
                clientContext.Web.RemoveAdministrator(userEntity);
            }
        }
        #endregion

        #region Group tests
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
        public void GroupExistsTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                bool groupExists = clientContext.Web.GroupExists(_testGroupName);
                Assert.IsTrue(groupExists);

                groupExists = clientContext.Web.GroupExists(_testGroupName + "987654321654367");
                Assert.IsFalse(groupExists);
            }
        }

        #endregion

        #region Permission level tests
        [TestMethod]
        public void AddPermissionLevelToGroupTest()
        {
            using (ClientContext clientContext = TestCommon.CreateClientContext())
            {
                // Test
                clientContext.Web.AddPermissionLevelToGroup(_testGroupName, RoleType.Contributor, false);

                //Get Group
                Group group = clientContext.Web.SiteGroups.GetByName(_testGroupName);
                clientContext.ExecuteQueryRetry();

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
                clientContext.ExecuteQueryRetry();

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
                clientContext.ExecuteQueryRetry();

                //Assert
                Assert.IsTrue(CheckPermissionOnPrinciple(web, user, roleType));

                //Teardown: Expicitly remove given permission. 
                web.RemovePermissionLevelFromUser(_userLogin, roleType);
            }
        }

        [TestMethod]
        public void AddPermissionLevelToUserTestByRoleDefTest()
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
                clientContext.ExecuteQueryRetry();

                //Assert
                Assert.IsTrue(CheckPermissionOnPrinciple(web, user, "Approve"));

                //Teardown: Expicitly remove given permission. 
                web.RemovePermissionLevelFromUser(_userLogin, "Approve");
            }
        }
        #endregion

        #region Reader access tests
#if !CLIENTSDKV15
        [TestMethod]
        public void AddReaderAccessToEveryoneExceptExternalsTest()
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
                    clientContext.ExecuteQueryRetry();
                }
            }
        }
#endif

        [TestMethod]
        public void AddReaderAccessToEveryoneTest()
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
                    clientContext.ExecuteQueryRetry();
                }
            }
        }
        #endregion

        #region helper methods
        private bool CheckPermissionOnPrinciple(Web web, Principal principle, RoleType roleType)
        {
            //Get Roles for the User
            RoleDefinitionBindingCollection roleDefinitionBindingCollection =
                web.RoleAssignments.GetByPrincipal(principle).RoleDefinitionBindings;
            web.Context.Load(roleDefinitionBindingCollection);
            web.Context.ExecuteQueryRetry();

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
            web.Context.ExecuteQueryRetry();

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
        #endregion
    }
}