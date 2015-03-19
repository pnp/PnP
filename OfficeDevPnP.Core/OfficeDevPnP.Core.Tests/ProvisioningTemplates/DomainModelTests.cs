using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning;
using System.Xml.Linq;
using OfficeDevPnP.Core.Utilities;
using System.IO;

namespace OfficeDevPnP.Core.Tests.ProvisioningTemplates
{
    [TestClass]
    public class DomainModelTests
    {
        [TestMethod]
        [TestCategory("Provisioning Template Domain Model")]
        public void CanSerializeDomainObject()
        {
            ProvisioningTemplate _st;
            var path = string.Format(@"{0}\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningTemplate.xml");
            XDocument _doc = XDocument.Load(path);
            _st = XMLSerializer.Deserialize<ProvisioningTemplate>(_doc);
            Assert.IsNotNull(_st);

        }
    }
}
