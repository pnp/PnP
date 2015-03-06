using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Provisioning.Core.Configuration.Template;
using Framework.Provisioning.Core.Configuration;

namespace ProvisioningTests._6_Provider
{
    [TestClass]
    public class ProviderTests
    {
        [TestMethod]
        [TestCategory("Post Provider Tests")]
        public void CanGetPostProviderFromConfig()
        {
            var _expectedProviderCount = 2;

            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByID("CT1");
            var _siteTemplate = _template.GetSiteTemplate();
            var _providers = _siteTemplate.Providers;
            Assert.AreEqual(_expectedProviderCount, _providers.Count);
        }

        [TestMethod]
        [TestCategory("Post Provider Tests")]
        public void CanGetPostProviderCData()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _tf = _configFactory.GetTemplateFactory();
            TemplateManager _tm = _tf.GetTemplateManager();
            Template _template = _tm.GetTemplateByID("CT1");
            var _siteTemplate = _template.GetSiteTemplate();
            var _providers = _siteTemplate.Providers;
        }
    }
}
