using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Tests;
using OfficeDevPnP.Core.Entities;
using System.Configuration;
using Microsoft.Online.SharePoint.TenantAdministration;
namespace Microsoft.SharePoint.Client.Tests
{
    [TestClass()]
    public class SecurityExtensionsTests
    {
        private string _userLogin;
        [TestInitialize()]
        public void Initialize()
        {
            _userLogin = ConfigurationManager.AppSettings["SPOUserName"];
        }
        [TestMethod()]
        public void GetAdministratorsTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                Assert.IsTrue(clientContext.Web.GetAdministrators().Any(), "No administrators returned");
            }
        }

        [TestMethod()]
        public void AddAdministratorsTest()
        {
            // Difficult to test on a developer (MSDN) tenant, as there is only one user allowed.
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Count admins
                var initialCount = clientContext.Web.GetAdministrators().Count;
                var userEntity = new UserEntity() { LoginName = _userLogin, Email = _userLogin };
                clientContext.Web.AddAdministrators(new List<UserEntity>() { userEntity }, false);

                var newCount = clientContext.Web.GetAdministrators().Count;
                Assert.IsTrue(initialCount == newCount); // Assumes that we're on a dev tenant, and that the existing sitecol admin is the same as the user being added.

                clientContext.Web.RemoveAdministrator(userEntity);
            }
        }

        [TestMethod()]
        public void AddGroupTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Test
                var group = clientContext.Web.AddGroup("Test Group", "Test Description", true);
                Assert.IsInstanceOfType(group, typeof(Group), "Group object returned not of correct type");
                Assert.IsTrue(group.Title == "Test Group", "Group not created with correct title");

                // Cleanup
                if (group != null)
                    clientContext.Web.RemoveGroup(group);
            }
        }

        [TestMethod()]
        public void AddPermissionLevelToGroupTest()
        {

            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                var group = clientContext.Web.AddGroup("Test Group", "Test Description", true);

                // Test
                clientContext.Web.AddPermissionLevelToGroup("Test Group", RoleType.Contributor, false);

                // Cleanup

                clientContext.Web.RemoveGroup("Test Group");
            }
        }

        [TestMethod()]
        public void AddPermissionLevelToUserTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                Web web = clientContext.Web;

                RoleType roleType = RoleType.Contributor;

                //Setup: Make sure permission does not already exist
                web.RemovePermissionLevelFromUser(_userLogin, roleType);

                //Add Permission
                web.AddPermissionLevelToUser(_userLogin, roleType);

                //Get User
                User user = web.SiteUsers.GetByEmail(_userLogin);
                clientContext.Load(user);
                clientContext.ExecuteQuery();

                //Get Roles for the User
                RoleDefinitionBindingCollection roleDefinitionBindingCollection = web.RoleAssignments.GetByPrincipal(user).RoleDefinitionBindings;
                clientContext.Load(roleDefinitionBindingCollection);
                clientContext.ExecuteQuery();

                //Check if assigned role is found
                bool roleExists = false;
                foreach (RoleDefinition rd in roleDefinitionBindingCollection)
                {
                    if (rd.RoleTypeKind == roleType)
                    {
                        roleExists = true;
                    }
                }

                //Assert
                Assert.IsTrue(roleExists);

                //Teardown: Expicitly remove given permission. 
                web.RemovePermissionLevelFromUser(_userLogin, roleType);
            }
        }

        [TestMethod()]
        public void AddReaderAccessTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                User userIdentity = null;

                // Test
                userIdentity = clientContext.Web.AddReaderAccess();

                Assert.IsNotNull(userIdentity, "No user added");
                var existingUser = clientContext.Web.AssociatedVisitorGroup.Users.GetByLoginName(userIdentity.LoginName);
                
                Assert.IsNotNull(existingUser, "No user returned");
                Assert.IsInstanceOfType(existingUser, typeof(User), "Object returned not of correct type");

                // Cleanup
                if (existingUser != null)
                {
                    clientContext.Web.AssociatedVisitorGroup.Users.Remove(existingUser);
                    clientContext.Web.AssociatedVisitorGroup.Update();
                    clientContext.ExecuteQuery();
                }
            }
        }

        [TestMethod()]
        public void AddReaderAccessTest1()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                var userIdentity = "c:0(.s|true";

                // Test
                clientContext.Web.AddReaderAccess(OfficeDevPnP.Core.Enums.BuiltInIdentity.Everyone);

                var existingUser = clientContext.Web.AssociatedVisitorGroup.Users.GetByLoginName(userIdentity);
                Assert.IsNotNull(existingUser, "No user returned");
                Assert.IsInstanceOfType(existingUser, typeof(User), "Object returned not of correct type");

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
