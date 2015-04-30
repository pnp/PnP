using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Configuration;
using Provisioning.Common.Utilities;

using ProvisioningTests;
using Provisioning.Common.Configuration.Application;
using System.Configuration;

namespace ProvisioningTests._1_Configuration.AppSettingTests
{
    [TestClass]
    public class AppSettingTests
    {
       
        [TestMethod]
        [TestCategory("APP Settings Test")]
        public void AppSettingsCanGetAppSettingsManager()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _manager = _configFactory.GetAppSetingsManager();

            Assert.IsNotNull(_manager);
            Assert.IsInstanceOfType(_manager, typeof(IAppSettingsManager));
        }

        [TestMethod]
        [TestCategory("APP Settings Test")]
        public void AppSettingsCanReadAppSettingsConfiguration()
        {
            //TODO CHANGE THE TEST CONSTANTS TO MATCH YOUR ENVIRONEMNT
            var _configFactory = ConfigurationFactory.GetInstance();
            var _manager = _configFactory.GetAppSetingsManager();
            var settings = _manager.GetAppSettings();

            Assert.AreEqual(TestConstants.EXPECTED_SPHOST, settings.SPHostUrl);
            Assert.AreEqual(TestConstants.EXPECTED_SUPPORTTEAMNOTIFICATIONEMAIL, settings.SupportEmailNotification);
            Assert.AreEqual(TestConstants.EXPECTED_TENANTADMINURL, settings.TenantAdminUrl);
        }

        [TestMethod]
        [TestCategory("Configuration")]

        public void CanGetModulesSections()
        {
            ConfigManager _cm = new ConfigManager();
            var _mes = _cm.ModulesElements;
            var module = _mes[ModuleKeys.REPOSITORYMANGER_KEY];
            Assert.IsNotNull(_mes);
        }

        [TestMethod]
        [TestCategory("Configuration")]
        public void CanGetModulesByKey()
        {
            ConfigManager _cm = new ConfigManager();
            var module = _cm.GetModuleByName(ModuleKeys.REPOSITORYMANGER_KEY);
            Assert.IsNotNull(module.Name);
            Assert.IsNotNull(module);
        }
    }
}
