using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Utilities;

using ProvisioningTests;
using Framework.Provisioning.Core.Configuration.Application;

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

            Assert.AreEqual(TestConstants.EXPECTED_CLIENTID, settings.ClientID);
            Assert.AreEqual(TestConstants.EXPECTED_CLIENTSECRET, settings.ClientSecret);
            Assert.AreEqual(TestConstants.EXPECTED_SPHOST, settings.SPHostUrl);
            Assert.AreEqual(TestConstants.EXPECTED_SUPPORTTEAMNOTIFICATIONEMAIL, settings.SupportEmailNotification);
            Assert.AreEqual(TestConstants.EXPECTED_TENANTADMINURL, settings.TenantAdminUrl);
        }
    }
}
