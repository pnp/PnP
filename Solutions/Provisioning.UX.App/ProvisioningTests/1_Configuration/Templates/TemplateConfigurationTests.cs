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
            Template _template = _tm.GetTemplateByID("CT1");
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
        public void TemplateManagerCanGetBrandingPackageNameFromTemplateObject()
        {
            var _expectedTPN = "Garage";
            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByName("CT2");
            Assert.IsNotNull(_template);
            var _actualTPN = _template.BrandingPackage;
            Assert.AreEqual<string>(_expectedTPN, _actualTPN);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetBrandingPackageByName()
        {
            var _expectedTPN = "Contoso";
            var _expectedColorFile = "Resources/Themes/Contoso/contoso.spcolor";
            var _expectedFontFile = "Resources/Themes/Contoso/contoso.spfont";
            var _expectedBackgroundFile = "Resources/Themes/Contoso/contosobg.jpg";
            var _expectedMasterPage = "seattle.master";
            var _expectedAlternateCSS = "Resources/Themes/Contoso/contosocss.css";
            var _expectedsiteLogo="Contoso";
            var _expectedVersion = 1;


            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByName("CT1");
            Assert.IsNotNull(_template);

            var _actualTPN = _template.BrandingPackage;
            Assert.AreEqual<string>(_expectedTPN, _actualTPN);

            var _tp = _tm.GetBrandingPackageByName(_actualTPN);
            Assert.AreEqual<string>(_expectedColorFile, _tp.ColorFile);
            Assert.AreEqual<string>(_expectedFontFile, _tp.FontFile);
            Assert.AreEqual<string>(_expectedBackgroundFile, _tp.BackgroundFile);
            Assert.AreEqual<string>(_expectedMasterPage, _tp.MasterPage);
            Assert.AreEqual<string>(_expectedAlternateCSS, _tp.AlternateCSS);
            Assert.AreEqual<string>(_expectedsiteLogo, _tp.SiteLogo);
            Assert.AreEqual<int>(_expectedVersion, _tp.Version);
        }
        
        [TestMethod]
        [TestCategory("Template Configuration")]
        public void TemplateManagerCanGetBrandingPackageShouldBeNull()
        {
            var _cf = ConfigurationFactory.GetInstance();
            var _tf = _cf.GetTemplateFactory();
            var _tm = _tf.GetTemplateManager();
            var _tp = _tm.GetBrandingPackageByName("BLAH");
            Assert.IsNull(_tp);
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

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void CanGetSiteTemplateObject()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByID("CT1");
            var siteTemplate = _template.GetSiteTemplate();

            Assert.IsNotNull(siteTemplate);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void SiteTemplateCanGetContentTypes()
        {
            var _expectCount = 1;

            IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
            ITemplateFactory _templateFactory = _configFactory.GetTemplateFactory();
            var _templateManager = _templateFactory.GetTemplateManager();
            var _template = _templateManager.GetTemplateByName("CT1");
            var _siteTemplate = _template.GetSiteTemplate();
            var _actualContentTypes = _siteTemplate.ContentTypes;
            Assert.AreEqual(_expectCount, _actualContentTypes.Count);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void SiteTemplateCanGetContentTypesXMLSchema()
        {
            IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
            ITemplateFactory _templateFactory = _configFactory.GetTemplateFactory();
            var _templateManager = _templateFactory.GetTemplateManager();
            var _template = _templateManager.GetTemplateByName("CT1");
            var _siteTemplate = _template.GetSiteTemplate();
            var _firstContentType = _siteTemplate.ContentTypes[0];

            Assert.IsTrue(!string.IsNullOrEmpty(_firstContentType.SchemaXml));
        }


        [TestMethod]
        [TestCategory("Template Configuration")]
        public void SiteTemplateCanGetFieldsFromTemplateObject()
        {
            var _expectedCount = 4;
            IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
            ITemplateFactory _templateFactory = _configFactory.GetTemplateFactory();

            var _templateManager = _templateFactory.GetTemplateManager();
            var _template = _templateManager.GetTemplateByName("CT1");
            var _siteTemplate = _template.GetSiteTemplate();
            var _acctualCount = _siteTemplate.SiteFields.Count;
            Assert.AreEqual(_expectedCount, _acctualCount);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void CanGetSiteTemplateFeatures()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByID("CT1");
            var siteTemplate = _template.GetSiteTemplate();
            var features = siteTemplate.Features;

            var _expectedSiteFeatures = 3;
            var _expectedWebFeatures = 4;
            Assert.AreEqual(_expectedSiteFeatures, features.SiteFeatures.Count);
            Assert.AreEqual(_expectedWebFeatures, features.WebFeatures.Count);
        }

        [TestMethod]
        [TestCategory("Template Configuration")]
        public void CanGetSiteTemplateCustomActions()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByID("CT1");
            var siteTemplate = _template.GetSiteTemplate();
            var customActions = siteTemplate.CustomActions;

            var _expectedSiteCA = 3;
            var _expectedWebCA = 3;
            Assert.AreEqual(_expectedSiteCA, customActions.SiteCustomActions.Count);
            Assert.AreEqual(_expectedWebCA, customActions.WebCustomActions.Count);
        }
    }
}
