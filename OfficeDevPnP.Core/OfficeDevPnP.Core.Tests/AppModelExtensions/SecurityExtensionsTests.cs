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
            // Untestable with a dev tenant...

            using (var clientContext = TestCommon.CreateClientContext())
            {
                clientContext.Web.AddPermissionLevelToUser(_userLogin, RoleType.Administrator);
            }
        }

        [TestMethod()]
        public void AddReaderAccessTest()
        {
            using (var clientContext = TestCommon.CreateClientContext())
            {
                // Setup
                var userIdentity = string.Format("c:0-.f|rolemanager|spo-grid-all-users/{0}", clientContext.Web.GetAuthenticationRealm());
                
                
                // Test
                clientContext.Web.AddReaderAccess();

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
