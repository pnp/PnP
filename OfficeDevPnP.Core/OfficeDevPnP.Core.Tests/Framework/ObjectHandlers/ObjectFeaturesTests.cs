using System;
using System.Collections.Generic;
using System.Linq;
using System.ServiceModel.Configuration;
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
    public class ObjectFeaturesTests
    {
        private Guid featureId = Guid.Parse("{87294c72-f260-42f3-a41b-981a2ffce37a}");
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                if (!ctx.Web.IsFeatureActive(featureId))
                {
                    ctx.Web.ActivateFeature(featureId);
                }
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();
            template.Features.WebFeatures.Add(
                new OfficeDevPnP.Core.Framework.Provisioning.Model.Feature() 
                { ID = featureId, Deactivate = true});


            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser parser = new TokenParser(ctx.Web,template);
                new ObjectFeatures().ProvisionObjects(ctx.Web, template, parser);

                var f = ctx.Web.IsFeatureActive(featureId);

                Assert.IsFalse(f);
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
                template = new ObjectFeatures().CreateEntities(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.Features.SiteFeatures.Any());
                Assert.IsTrue(template.Features.WebFeatures.Any());
                Assert.IsInstanceOfType(template.Features, typeof(Features));
            }
        }
    }
}
