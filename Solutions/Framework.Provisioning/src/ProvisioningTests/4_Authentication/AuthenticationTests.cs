using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Provisioning.Core.Authentication;
using Framework.Provisioning.Core.Configuration;
using Framework.Provisioning.Core.Configuration.Application;

namespace ProvisioningTests._4_Authentication
{
    [TestClass]
    public class AuthenticationTests
    {
        [TestMethod]
        [TestCategory("Authentication Tests")]
        public void AuthenticationCanGetAuthenticatedContextTest()
        {
            var appOnlyAuth = this.CreateAppOnlyAuthenticationObject();
            var ctx = appOnlyAuth.GetAuthenticatedContext();
            Assert.IsNotNull(ctx);
        }

        [TestMethod]
        [TestCategory("Authentication Tests")]
        public void AuthenticationCanGetWebConfigurationIfEmpty()
        {
            var appOnlyAuth = new AppOnlyAuthenticationTenant();
            Assert.AreEqual<string>(TestConstants.EXPECTED_CLIENTID, appOnlyAuth.AppId);
            Assert.AreEqual<string>(TestConstants.EXPECTED_CLIENTSECRET, appOnlyAuth.AppSecret);
            Assert.AreEqual<string>(TestConstants.EXPECTED_TENANTADMINURL, appOnlyAuth.TenantAdminUrl);
        }

        [TestMethod]
        [TestCategory("Authentication Tests")]
        public void AuthenticationCanUseAuthenticatedClientContextFromConfig()
        {
            var appOnlyAuth = new AppOnlyAuthenticationTenant();
            using (var ctx = appOnlyAuth.GetAuthenticatedContext())
            {
                var web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                Assert.IsNotNull(web.Title);
            }

            using (var ctx = appOnlyAuth.GetAuthenticatedContext())
            {
                var web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                Assert.IsNotNull(web.Title);
            }          
        }

        [TestMethod]
        [TestCategory("Authentication Tests")]
        public void AuthenticationCanUseAuthenticatedClientContextFromObject()
        {
            var appOnlyAuth = this.CreateAppOnlyAuthenticationObject();
            using (var ctx = appOnlyAuth.GetAuthenticatedContext())
            {
                var web = ctx.Web;
                ctx.Load(web);
                ctx.ExecuteQuery();
                Assert.IsNotNull(web.Title);
            }          
        }
     
        #region TestSupport
        AppOnlyAuthenticationTenant CreateAppOnlyAuthenticationObject()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _manager = _configFactory.GetAppSetingsManager();
            var _settings = _manager.GetAppSettings();

            AppOnlyAuthenticationTenant target = new AppOnlyAuthenticationTenant()
            {
                TenantAdminUrl = _settings.TenantAdminUrl,
                AppId = _settings.ClientID,
                AppSecret = _settings.ClientSecret
            };
            return target;
        }
        #endregion
    }
}
