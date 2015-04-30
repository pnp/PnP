using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;


using Provisioning.Common;
using Microsoft.SharePoint.Client;
using Provisioning.Common.Configuration;
using Provisioning.Common.Data;
using Provisioning.Common.Data.SiteRequests;

namespace ProvisioningTests._3_SiteRequests
{
    [TestClass]
    public class SiteRequestFactoryTests
    {
        ClientContext _validClientCtx;

        [TestInitialize]
        public void TestInit()
        {
            var _configFactory = ConfigurationFactory.GetInstance();
            var _manager = _configFactory.GetAppSetingsManager();
            var _settings = _manager.GetAppSettings();

            Uri _siteURI = new Uri(_settings.SPHostUrl);

            string _realm = TokenHelper.GetRealmFromTargetUrl(_siteURI);
            string _accessToken = TokenHelper.GetAppOnlyAccessToken(
                TokenHelper.SharePointPrincipal,
                _siteURI.Authority,
                _realm).AccessToken;

            _validClientCtx = TokenHelper.GetClientContextWithAccessToken(_siteURI.ToString(), _accessToken);
        }

        [TestMethod]
        [TestCategory("Site Factory Tests")]
        public void SiteRequestFactoryCanGetInstance()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();  
            Assert.IsNotNull(_actualFactory);
            Assert.IsInstanceOfType(_actualFactory, typeof(ISiteRequestFactory));
        }

        [TestMethod]
        [TestCategory("Site Factory Tests")]
        public void SiteRequestFactoryCanGetDefaultManager()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();

            Assert.IsInstanceOfType(_manager, typeof(ISiteRequestManager));
            Assert.IsNotNull(_manager);
        }
       
        [TestMethod]
        [TestCategory("Site Factory Tests")]
        public void SiteRequestFactoryCanGetSiteRequestListID()
        {
            Web _web = this._validClientCtx.Web;

            var _listID = SiteRequestList.CreateSharePointRepositoryList(_web, 
                SiteRequestList.TITLE, 
                SiteRequestList.DESCRIPTION, 
                SiteRequestList.LISTURL);
            Assert.IsNotNull(_listID);
        }

        #region Test Support
        public ClientContext CreateInvalidClientContext()
        {
            ClientContext _ctx = new ClientContext("https://invalid.com");
            return _ctx;
        }
        #endregion
    }
}
