using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Configuration;
using Provisioning.Common.Utilities;

using ProvisioningTests;
using Provisioning.Common.Configuration.Application;
using System.Configuration;
using System.Text.RegularExpressions;

namespace ProvisioningTests._1_Configuration
{
    [TestClass]
    public class ConfigurationTests
    {
        [TestMethod]
        [TestCategory("Configuration")]
        public void AppSettingsCanGetAppSettingsManager()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _manager = _configFactory.GetAppSetingsManager();

            Assert.IsNotNull(_manager);
            Assert.IsInstanceOfType(_manager, typeof(IAppSettingsManager));
        }

        [TestMethod]
        [TestCategory("Configuration")]
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
        
        [TestMethod]
        [TestCategory("Configuration")]
        public void CanParseEnvironmentToken()
        {
            ConfigManager _cm = new ConfigManager();
            var _expected = "C:\\ProgramData/Resources/SiteTemplate";
            var _actual = _cm.GetAppSettingsKey("ENVRTEST1");

            Regex r = new Regex(@"(?:(?<=\().+?(?=\))|(?<=\[).+?(?=\]))");
            Regex r1 = new Regex(@"\[(.*?)\]");

            Match _outPut = r.Match(_actual);
            if(_outPut.Success)
            {
                var _env = Environment.GetEnvironmentVariable(_outPut.Value);
                _actual = r1.Replace(_actual, _env);
            }
            Assert.AreEqual(_expected, _actual);
        }

        [TestMethod]
        [TestCategory("Configuration")]
        public void CanParseEnvironmentTokenFromConfigManager()
        {
            ConfigManager _cm = new ConfigManager();
            var _expected = "C:\\ProgramData/Resources/SiteTemplate";
            var _actual = _cm.GetAppSettingsKey("ENVRTEST1");
            Assert.AreEqual(_expected, _actual);
        }

        [TestMethod]
        [TestCategory("Configuration")]
        public void CanParseConfigWithNoEnvironmentFromConfigManager()
        {
            ConfigManager _cm = new ConfigManager();
            var _expected = "Resources/SiteTemplate";
            var _actual = _cm.GetAppSettingsKey("ENVRTEST2");
            Assert.AreEqual(_expected, _actual);
        }
        
    }
}
