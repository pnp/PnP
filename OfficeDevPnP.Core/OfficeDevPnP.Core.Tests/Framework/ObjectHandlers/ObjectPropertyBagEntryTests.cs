using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Microsoft.SharePoint.Client;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

        [TestInitialize]
        public void Initialize()
        {
            key = string.Format("Test_{0}", DateTime.Now.Ticks);
        }
        [TestCleanup]
        public void CleanUp()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                ctx.Web.RemovePropertyBagValue(key);
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
                new ObjectPropertyBagEntry().ProvisionObjects(ctx.Web, template);

                var value = ctx.Web.GetPropertyBagValueString(key, "default");
                Assert.IsTrue(value == "Unit Test");
            }
        }

        [TestMethod]
        public void CanCreateEntities()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var template = new ProvisioningTemplate();
                template = new ObjectPropertyBagEntry().CreateEntities(ctx.Web, template);

                Assert.IsTrue(template.PropertyBagEntries.Any());
            }
        }
    }
}
