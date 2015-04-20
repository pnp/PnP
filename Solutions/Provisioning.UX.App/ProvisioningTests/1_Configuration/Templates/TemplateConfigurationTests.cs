using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Configuration.Template;
using System.Xml;
using Provisioning.Common.Utilities;
using Provisioning.Common.Configuration;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ProvisioningTests._1_Configuration.Templates
{
    [TestClass]
    public class TemplateConfigurationTests
    {
        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetProvisionTemplates()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Assert.IsNotNull(_tf);
            Assert.IsInstanceOfType(_tf, typeof(ITemplateFactory));
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetProvisionTemplatesByID()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByName("CT1");
            Assert.IsNotNull(_template);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetProvisionTemplatesByName()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByName("CT1");
            Assert.IsNotNull(_template);
        }
       
        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetProvisionTemplatesByNameNotFound()
        {
            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByName("BLAH");
            Assert.IsNull(_template);
        }
     
        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetAllEnableProvisioningTemplates()
        {
            int _expectCount = 2;

            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            var _templates = _tm.GetAvailableTemplates();
            
            Assert.IsNotNull(_templates);
            Assert.AreEqual(_expectCount, _templates.Count);
  
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetAllSubSiteTemplates()
        {
            int _expectedCount = 1;

            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            var _templates = _tm.GetSubSiteTemplates();

            Assert.AreEqual(_expectedCount, _templates.Count);

        }

     
        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanDeserializeTemplateConfig()
        {
            TemplateConfiguration _tc;
            XDocument _doc = XDocument.Load("Resources/Templates/Templates.config");
            _tc = XmlSerializerHelper.Deserialize<TemplateConfiguration>(_doc);
            var _templates = _tc.Templates;

            Assert.AreEqual(3, _templates.Count);
        } 
    }
}
