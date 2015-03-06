using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Provisioning.Core.Data;
using Framework.Provisioning.Core.Configuration;

namespace ProvisioningTests._1_Configuration.Templates
{
    [TestClass]
    public class CustomActionTests
    {
        [TestMethod]
        [TestCategory("Template Custom Action Tests")]
        public void CanGetCustomActions()
        {
            var _expectedCount = 3;
            ISiteRequestFactory _requestFactory = SiteRequestFactory.GetInstance();
            IConfigurationFactory _configFactory = ConfigurationFactory.GetInstance();
            ITemplateFactory _templateFactory = _configFactory.GetTemplateFactory();

            var _templateManager = _templateFactory.GetTemplateManager();
            var _customActions = _templateManager.GetCustomActions();

            Assert.AreEqual(_expectedCount, _customActions.Count);

        }
    }
}
