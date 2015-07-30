using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Entities;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;
using User = OfficeDevPnP.Core.Framework.Provisioning.Model.User;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectSiteSecurityTests
    {

        private List<UserEntity> admins;

        [TestInitialize]
        public void Initialize()
        {

            using (var ctx = TestCommon.CreateClientContext())
            {
                admins = ctx.Web.GetAdministrators();
            }
        }

        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var memberGroup = ctx.Web.AssociatedMemberGroup;
                ctx.Load(memberGroup);
                ctx.ExecuteQueryRetry();
                foreach (var user in admins)
                {
                    try
                    {
                        ctx.Web.RemoveUserFromGroup(memberGroup.Title, user.LoginName);
                    }
                    catch (ServerException)
                    {
                        
                    }
                }
            }
        }

        [TestMethod]
        public void CanProvisionAdditionalGroups()
        {
            var template = new ProvisioningTemplate();

            var additionalGroup1 = new AdditionalGroup(){ Name="Test Additional Group1", Description="Test AdditionalGroup1Description" };
            foreach (var user in admins)
            {
                additionalGroup1.Members.Add(new User() { Name = user.LoginName });
            }
            template.Security.AdditionalGroups.Add(additionalGroup1);

            var additionalGroup2 = new AdditionalGroup() { Name = "Test Additional Group2", Description = "Test AdditionalGroup2Description" };
            foreach (var user in admins)
            {
                additionalGroup2.Members.Add(new User() { Name = user.LoginName });
            }
            template.Security.AdditionalGroups.Add(additionalGroup2);
            

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser.Initialize(ctx.Web, template);
                new ObjectSiteSecurity().ProvisionObjects(ctx.Web, template);

                Assert.IsTrue(ctx.Web.GroupExists("Test Additional Group1"));
                Assert.IsTrue(ctx.Web.GroupExists("Test Additional Group2"));

                var group1 = ctx.Web.SiteGroups.GetByName("Test Additional Group1");
                ctx.Load(group1, g => g.Users);
                ctx.ExecuteQueryRetry();
                foreach (var user in admins)
                {
                    var existingUser = group1.Users.GetByLoginName(user.LoginName);
                    ctx.Load(existingUser);
                    ctx.ExecuteQueryRetry();
                    Assert.IsNotNull(existingUser);
                }

                var group2 = ctx.Web.SiteGroups.GetByName("Test Additional Group2"); 
                ctx.Load(group2, g => g.Users);
                ctx.ExecuteQueryRetry();
                foreach (var user in admins)
                {
                    var existingUser = group2.Users.GetByLoginName(user.LoginName);
                    ctx.Load(existingUser);
                    ctx.ExecuteQueryRetry();
                    Assert.IsNotNull(existingUser);
                }

            }
        }
        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();


            foreach (var user in admins)
            {
                template.Security.AdditionalMembers.Add(new User() { Name = user.LoginName});
            }



            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser.Initialize(ctx.Web, template);
                new ObjectSiteSecurity().ProvisionObjects(ctx.Web, template);

                var memberGroup = ctx.Web.AssociatedMemberGroup;
                ctx.Load(memberGroup, g => g.Users);
                ctx.ExecuteQueryRetry();
                foreach (var user in admins)
                {
                    var existingUser = memberGroup.Users.GetByLoginName(user.LoginName);
                    ctx.Load(existingUser);
                    ctx.ExecuteQueryRetry();
                    Assert.IsNotNull(existingUser);
                }
            }
        }

        [TestMethod]
        public void CanCreateEntities()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                // Load the base template which will be used for the comparison work
                var creationInfo = new ProvisioningTemplateCreationInformation(ctx.Web) { BaseTemplate = ctx.Web.GetBaseTemplate() };

                var template = new ProvisioningTemplate();
                template = new ObjectSiteSecurity().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.Security.AdditionalAdministrators.Any());
            }
        }
    }
}
