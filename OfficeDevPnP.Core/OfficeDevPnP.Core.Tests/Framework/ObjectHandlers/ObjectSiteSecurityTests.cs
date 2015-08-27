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
                new ObjectSiteSecurity().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation());

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
                template = new ObjectSiteSecurity().ExtractObjects(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.Security.AdditionalAdministrators.Any());
            }
        }
    }
}
