using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Xml;
using Provisioning.Common.Utilities;
using Provisioning.Common.Configuration;
using System.Collections.Generic;
using System.Xml.Linq;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using Provisioning.Common.Data.Templates;
using Provisioning.Common.Data.Templates.Impl;

namespace ProvisioningTests._3_Data
{
    [TestClass]
    public class TemplateConfigurationTests
    {
        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetTemplates()
        {
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
          
            Assert.IsNotNull(_tm);
            Assert.IsInstanceOfType(_siteFactory, typeof(ISiteTemplateFactory));
            Assert.IsInstanceOfType(_tm, typeof(ISiteTemplateManager));
        }

        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetTemplateByName()
        {
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
           
            Template _template = _tm.GetTemplateByName("TEMPLATE1");
            Assert.IsNotNull(_template);
        }

    
        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetTemplateByNameNotFound()
        {
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
            Template _template = _tm.GetTemplateByName("BLAH");
            Assert.IsNull(_template);
        }
     
        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetAllEnableTemplates()
        {
            int _expectCount = 2;

            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
            var _templates = _tm.GetAvailableTemplates();
            
            Assert.IsNotNull(_templates);
            Assert.AreEqual(_expectCount, _templates.Count);
  
        }

        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetAllSubSiteTemplates()
        {
            int _expectedCount = 1;
            var _siteFactory = SiteTemplateFactory.GetInstance();

            var _tm = _siteFactory.GetManager();
            var _templates = _tm.GetSubSiteTemplates();

            Assert.AreEqual(_expectedCount, _templates.Count);
        }

     
        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanDeserializeTemplateConfig()
        {
            XMLSiteTemplateData _tc;
            XDocument _doc = XDocument.Load("Resources/SiteTemplates/Templates.config");
            _tc = XmlSerializerManager.Deserialize<XMLSiteTemplateData>(_doc);
            var _templates = _tc.Templates;
            Assert.AreEqual(3, _templates.Count);
        }


        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetProvisionTemplateByName()
        {
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();
            Template _template = _tm.GetTemplateByName("TEMPLATE1");
            ProvisioningTemplate _pt = null;
           // XMLFileSystemTemplateProvider _ptProvider = new XMLFileSystemTemplateProvider(_template.ProvisioningTemplateContainer, string.Empty);
            _pt = _tm.GetProvisioningTemplate(_template.ProvisioningTemplate);
            Assert.IsNotNull(_pt);
        }

        [TestMethod]
        [TestCategory("Site Template Factory")]
        public void TemplateManagerCanGetProvisionTemplateByNameException()
        {
            var _siteFactory = SiteTemplateFactory.GetInstance();
            var _tm = _siteFactory.GetManager();

            ProvisioningTemplate _pt = null;
            _pt = _tm.GetProvisioningTemplate("IDONTEXIST");
            Assert.IsNull(_pt);
        }

    }
}
