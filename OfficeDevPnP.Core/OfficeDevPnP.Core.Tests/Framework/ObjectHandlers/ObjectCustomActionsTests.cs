using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectCustomActionsTests
    {
        private const string ActionName = "Test Custom Action";
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (ctx.Site.CustomActionExists("Test Custom Action"))
                {
                    var action = ctx.Site.GetCustomActions().FirstOrDefault(c => c.Name == ActionName);
                    action.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            var ca = new Core.Framework.Provisioning.Model.CustomAction();
            ca.Name = "Test Custom Action";
            ca.Location = "ScriptLink";
            ca.ScriptBlock = "alert('Hello PnP!');";

            template.CustomActions.SiteCustomActions.Add(ca);

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser parser = new TokenParser(ctx.Web);
                new ObjectCustomActions().ProvisionObjects(ctx.Web, template, parser);

                Assert.IsTrue(ctx.Site.CustomActionExists("Test Custom Action"));
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
                template = new ObjectCustomActions().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsInstanceOfType(template.CustomActions, typeof(CustomActions));
            }
        }
    }
}
