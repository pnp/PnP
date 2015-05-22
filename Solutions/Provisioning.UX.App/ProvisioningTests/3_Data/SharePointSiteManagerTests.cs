using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Data;
using Provisioning.Common;
using System.Collections.Generic;
using Provisioning.Common.Data.SiteRequests;

namespace ProvisioningTests._3_Data
{
    [TestClass]
    public class SharePointSiteManagerTests
    {
        [TestMethod]
        [TestCategory("Site Request")]
        public void SharePointSiteManagerCanCreateNewRequest()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            
            var _siteInfo = this.GetSiteRequestMock();
            _manager.CreateNewSiteRequest(_siteInfo);
        }

        [TestMethod]
        [TestCategory("Site Request")]
        public void SharePointSiteManagerCanGetApprovedRequests()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _newRequests = _manager.GetApprovedRequests();
        }

        [TestMethod]
        [TestCategory("Site Request")]
        public void SharePointSiteManagerCanGetNewRequests()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _newRequests =  _manager.GetNewRequests();
        }

        [TestMethod]
        [TestCategory("Site Request")]
        public void SharePointSiteManagerDoesRequestByUrl()
        {
            var _expectValue = true;
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _mock = this.GetSiteRequestMock();
            var _actual = _manager.DoesSiteRequestExist(_mock.Url);

            Assert.AreEqual(_expectValue, _actual);
        }

        [TestMethod]
        [TestCategory("Site Request")]
        public void SharePointSiteManagerCanGetRequestByUrl()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _expected = this.GetSiteRequestMock();
            var _actual = _manager.GetSiteRequestByUrl(_expected.Url);
            Assert.AreEqual(_expected.Url, _actual.Url);
           
        }

        public SiteRequestInformation GetSiteRequestMock()
        {
            var _owner = new SharePointUser()
            {
                Email = "frank@marascohome.com"
            };
            //Add addtional Users
            List<SharePointUser> _additionalAdmins = new List<SharePointUser>();
            SharePointUser _admin1 = new SharePointUser();
            //   _admin1.Email = "franktest@MicrosoftACS.onmicrosoft.com";
            _admin1.Email = "frank@marascohome.com";
            SharePointUser _admin2 = new SharePointUser();
            //  _admin2.Email = "frank@microsoftacs.onmicrosoft.com";
            _admin2.Email = "brianmic@marascohome.com";
            _additionalAdmins.Add(_admin1);
            _additionalAdmins.Add(_admin2);

            var _siteInfo = new SiteRequestInformation()
            {
                Title = "Test Title",
                Description = "Test Description",
                Template = "CT2",
                Url = "https://spsites.marascohome.com/sites/B3",
                SitePolicy = "HBI",
                SiteOwner = _owner,
                AdditionalAdministrators = _additionalAdmins,
                EnableExternalSharing = true,
                SharePointOnPremises = true
            };

            return _siteInfo;

        }
    }
}
