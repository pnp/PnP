using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Enums;
using OfficeDevPnP.Core.Framework.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.ObjectHandlers;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using ContentType = OfficeDevPnP.Core.Framework.Provisioning.Model.ContentType;

namespace OfficeDevPnP.Core.Tests.Framework.ObjectHandlers
{
    [TestClass]
    public class ObjectContentTypeTests
    {
        private const string ElementSchema = @"<ContentType ID=""0x010100503B9E20E5455344BFAC2292DC6FE805"" Name=""Test Content Type"" Group=""PnP"" Version=""1"" xmlns=""http://schemas.microsoft.com/sharepoint/v3"" />";

        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var ct = ctx.Web.GetContentTypeByName("Test Content Type");
                if (ct != null)
                {
                    ct.DeleteObject();
                    ctx.ExecuteQueryRetry();
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();


            var contentType = new ContentType()
            {
                Id = "0x010100503B9E20E5455344BFAC2292DC6FE805",
                Name = "Test Content Type",
                Group = "PnP",
                Description = "Test Description",
                Overwrite = true,
                Hidden = false
            };

            contentType.FieldRefs.Add(new FieldRef()
            {
                Id = BuiltInFieldId.Category,
                DisplayName = "Test Category",
            });
            template.ContentTypes.Add(contentType);

            using (var ctx = TestCommon.CreateClientContext())
            {
                new ObjectContentType().ProvisionObjects(ctx.Web, template);

                var ct = ctx.Web.GetContentTypeByName("Test Content Type");

                Assert.IsNotNull(ct);

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
                template = new ObjectContentType().ExtractObjects(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.ContentTypes.Any());
                Assert.IsInstanceOfType(template.ContentTypes, typeof(List<ContentType>));
            }
        }
    }
}
