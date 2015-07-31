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
    public class ObjectFieldTests
    {
        private const string ElementSchema = @"<Field xmlns=""http://schemas.microsoft.com/sharepoint/v3"" StaticName=""DemoField"" DisplayName=""Test Field"" Type=""Text"" ID=""{7E5E53E4-86C2-4A64-9F2E-FDFECE6219E0}"" Group=""PnP"" Required=""true""/>";
        private Guid fieldId = Guid.Parse("{7E5E53E4-86C2-4A64-9F2E-FDFECE6219E0}");
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var f = ctx.Web.GetFieldById<FieldText>(fieldId); // Guid matches ID in field caml.
                if (f != null)
                {
                    f.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            template.SiteFields.Add(new Core.Framework.Provisioning.Model.Field() { SchemaXml = ElementSchema });

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser.Initialize(ctx.Web, template);
                new ObjectField().ProvisionObjects(ctx.Web, template);

                var f = ctx.Web.GetFieldById<FieldText>(fieldId);

                Assert.IsNotNull(f);
                Assert.IsInstanceOfType(f, typeof(FieldText));
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
                template = new ObjectField().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.SiteFields.Any());
                Assert.IsInstanceOfType(template.SiteFields, typeof(List<Core.Framework.Provisioning.Model.Field>));
            }
        }
    }
}
