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
    public class ObjectPropertyBagEntryTests
    {
        private string key;
        private string systemKey;

        [TestInitialize]
        public void Initialize()
        {
            key = string.Format("Test_{0}", DateTime.Now.Ticks);
            systemKey = string.Format("vti_test_{0}", DateTime.Now.Ticks);
        }
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ctx.Web.RemovePropertyBagValue(key);
                ctx.Web.RemovePropertyBagValue(systemKey);
            }
        }

        [TestMethod]
        public void CanProvisionObjects()
        {
            var template = new ProvisioningTemplate();

            var propbagEntry = new Core.Framework.Provisioning.Model.PropertyBagEntry();
            propbagEntry.Key = key;
            propbagEntry.Value = "Unit Test";

            template.PropertyBagEntries.Add(propbagEntry);

            using (var ctx = TestCommon.CreateClientContext())
            {
                TokenParser.Initialize(ctx.Web, template);
                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation());

                var value = ctx.Web.GetPropertyBagValueString(key, "default");
                Assert.IsTrue(value == "Unit Test");

                // Create same entry, but don't overwrite.
                template = new ProvisioningTemplate();

                var propbagEntry2 = new PropertyBagEntry();
                propbagEntry2.Key = key;
                propbagEntry2.Value = "Unit Test 2";
                propbagEntry2.Overwrite = false;

                template.PropertyBagEntries.Add(propbagEntry2);

                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation());

                value = ctx.Web.GetPropertyBagValueString(key, "default");
                Assert.IsTrue(value == "Unit Test");


                // Create same entry, but overwrite
                template = new ProvisioningTemplate();

                var propbagEntry3 = new PropertyBagEntry();
                propbagEntry3.Key = key;
                propbagEntry3.Value = "Unit Test 3";
                propbagEntry3.Overwrite = true;

                template.PropertyBagEntries.Add(propbagEntry3);

                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation());

                value = ctx.Web.GetPropertyBagValueString(key, "default");
                Assert.IsTrue(value == "Unit Test 3");

                // Create entry with system key. We don't specify to overwrite system keys, so the key should not be created.
                template = new ProvisioningTemplate();

                var propbagEntry4 = new PropertyBagEntry();
                propbagEntry4.Key = systemKey;
                propbagEntry4.Value = "Unit Test System Key";
                propbagEntry4.Overwrite = true;

                template.PropertyBagEntries.Add(propbagEntry4);

                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation());

                value = ctx.Web.GetPropertyBagValueString(systemKey, "default");
                Assert.IsTrue(value == "default");

                // Create entry with system key. We _do_ specify to overwrite system keys, so the key should be created.
                template = new ProvisioningTemplate();

                var propbagEntry5 = new PropertyBagEntry();
                propbagEntry5.Key = systemKey;
                propbagEntry5.Value = "Unit Test System Key 5";
                propbagEntry5.Overwrite = true;

                template.PropertyBagEntries.Add(propbagEntry5);

                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation() { OverwriteSystemPropertyBagValues = true});

                value = ctx.Web.GetPropertyBagValueString(systemKey, "default");
                Assert.IsTrue(value == "Unit Test System Key 5");

                // Create entry with system key. We _do not_ specify to overwrite system keys, so the key should not be created.
                template = new ProvisioningTemplate();

                var propbagEntry6 = new PropertyBagEntry();
                propbagEntry6.Key = systemKey;
                propbagEntry6.Value = "Unit Test System Key 6";
                propbagEntry6.Overwrite = true;

                template.PropertyBagEntries.Add(propbagEntry6);

                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template, new ProvisioningTemplateApplyingInformation() { OverwriteSystemPropertyBagValues = false });

                value = ctx.Web.GetPropertyBagValueString(systemKey, "default");
                Assert.IsFalse(value == "Unit Test System Key 6");
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
                template = new ObjectPropertyBagEntry().ExtractObjects(ctx.Web, template, creationInfo);

                Assert.IsTrue(template.PropertyBagEntries.Any());
            }
        }
    }
}
