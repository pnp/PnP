using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Data;
using Provisioning.Common;
using System.Collections.Generic;
using Provisioning.Common.Data.SiteRequests;

namespace ProvisioningTests._3_Data
{
    [TestClass]
    public class SiteRequestManagerTests
    {
        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerCanCreateNewRequest()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            
            var _siteInfo = this.GetSiteRequestMock();
            _manager.CreateNewSiteRequest(_siteInfo);
        }

        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerCanGetApprovedRequests()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _newRequests = _manager.GetApprovedRequests();
        }

        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerCanGetNewRequests()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _newRequests =  _manager.GetNewRequests();
        }

        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerDoesRequestByUrl()
        {
            var _expectValue = true;
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _mock = this.GetSiteRequestMock();
            var _actual = _manager.DoesSiteRequestExist(_mock.Url);

            Assert.AreEqual(_expectValue, _actual);
        }

        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestCanGetRequestByUrl()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _expected = this.GetSiteRequestMock();
            var _actual = _manager.GetSiteRequestByUrl(_expected.Url);
            Assert.AreEqual(_expected.Url, _actual.Url);
        }
        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerCanGetOwnerRequestsByEmail()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _expected = this.GetSiteRequestMock();

            var _actualRequests = _manager.GetOwnerRequests(_expected.SiteOwner.Name);
            foreach(var _actualRequest in _actualRequests)
            {
                Assert.AreEqual(_actualRequest.SiteOwner.Name, _expected.SiteOwner.Name);
            }
        }

        [TestMethod]
        [TestCategory("Site Request Manager")]
        public void SiteRequestManagerCanUpdateRequestStatus()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            var _manager = _actualFactory.GetSiteRequestManager();
            var _expected = this.GetSiteRequestMock();
            _manager.UpdateRequestStatus(_expected.Url, SiteRequestStatus.Approved);
            var _actual = _manager.GetSiteRequestByUrl(_expected.Url);
            Assert.AreEqual<string>(_actual.RequestStatus, SiteRequestStatus.Approved.ToString());
            Assert.AreEqual(_expected.Url, _actual.Url);
        }

        public SiteInformation GetSiteRequestMock()
        {
            var _owner = new SiteUser()
            {
                Name = "frank@marascohome.com"
            };
            //Add addtional Users
            List<SiteUser> _additionalAdmins = new List<SiteUser>();
            SiteUser _admin1 = new SiteUser();
            _admin1.Name = "frank@marascohome.com";
            SiteUser _admin2 = new SiteUser();
            _admin2.Name = "frank@marascohome.com";
            _additionalAdmins.Add(_admin1);
            _additionalAdmins.Add(_admin2);

            var _siteInfo = new SiteInformation()
            {
                Title = "Test Title",
                Description = "Test Description",
                Template = "CT2",
                Url = "https://spsites.contoso.com/sites/B3",
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
