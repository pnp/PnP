using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Patterns.Provisioning.Common.Data;
using Microsoft.SharePoint.Client;
using System.Security;
using Patterns.Provisioning.Common;
using System.Collections.Generic;

namespace Patterns.Provisioning.Tests.Common.Data
{
    [TestClass]
    public class DataTest
    {
        /// <summary>
        ///A test for DataStoreContext Constructor
        ///</summary>
        [TestMethod()]
        public void SiteRequestFactoryTest()
        {
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            Assert.IsNotNull(_actualFactory);
        }
        
        /// <summary>
        /// Test Method to work with the SharePoint Site DataSource Repository
        /// </summary>
         [TestMethod()]
        public void SpSiteRepositoryTestThrowsExceptionListParam()
        {
            ClientContext _ctx = GetValidContext();
            ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
            _ctx.ExecuteQuery();
            ISiteRequestManager _actual = _actualFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            Assert.IsNotNull(_actual);
          
        }

         /// <summary>
         /// Test Method to work with the SharePoint Site DataSource Repository
         /// </summary>
         [TestMethod()]
         [ExpectedException(typeof(ArgumentNullException))]
         public void SpSiteRepositoryTestThrowsExceptionCtxParamIsNull()
         {
             ClientContext _ctx = null;
             ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
             ISiteRequestManager _actual = _actualFactory.GetSPSiteRepository(_ctx, "SiteRequests");
         }

         /// <summary>
         /// Test Method to work with the SharePoint Site DataSource Repository. Test invalid Contenxt
         /// </summary>
         [TestMethod()]
         [ExpectedException(typeof(DataStoreException))]
         public void SpSiteRepositoryTestInvalidContext()
         {
             ClientContext _ctx = this.GetInvalidContext();
             ISiteRequestFactory _actualFactory = SiteRequestFactory.GetInstance();
             ISiteRequestManager _actual = _actualFactory.GetSPSiteRepository(_ctx, "SiteRequests");
         }

        /// <summary>
        /// Test Method to work with the SharePoint Site DataSource Repository
        /// </summary>
        [TestMethod()]
        public void SpSiteRepositoryManagerCanInsertNewRecord()
         {  
             ClientContext _ctx = GetValidContext();
 
             ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
             ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");

             var _owner = new SharePointUser()
             {
                 Email = "frank@microsoftacs.onmicrosoft.com"
             };

             //Add addtional Users
             List<SharePointUser> _additionalAdmins = new List<SharePointUser>();
             SharePointUser _admin1 = new SharePointUser();
             _admin1.Email = "Test1@MicrosoftACS.onmicrosoft.com";

             SharePointUser _admin2 = new SharePointUser();
             _admin2.Email = "Generic@MicrosoftACS.onmicrosoft.com";
             _additionalAdmins.Add(_admin1);
             _additionalAdmins.Add(_admin2);

             var _site = new SiteRequestInformation()
             {
                 Title = "Test Title",
                 Description = "Test Description",
                 Template = "STS#0", 
                 Url = "https://microsoftacs.onmicrosoft.com/teams/test",
                 SitePolicy = "HBI",
                 SiteOwner = _owner,
                 AdditionalAdministrators = _additionalAdmins,
                 Lcid = 1033

             };
             _siteRequestManager.CreateNewSiteRequest(_site);

         }

        [TestMethod]
        public void SpSiteRepositoryCanGetNewRecords()
        {
            ClientContext _ctx = GetValidContext();

            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            ICollection<SiteRequestInformation> _actual = _siteRequestManager.GetNewRequests();
            Assert.IsNotNull(_actual);
        }

        [TestMethod]
        public void SpSiteRepositoryCanGetRecordByUrl()
        {
            ClientContext _ctx = GetValidContext();
            var _targetURL = "https://microsoftacs.onmicrosoft.com/teams/test";
            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            SiteRequestInformation _siteInfo = _siteRequestManager.GetSiteRequestByUrl(_targetURL);
            Assert.IsNotNull(_siteInfo);
        }

        [TestMethod]
        [ExpectedException(typeof(ArgumentException))]
        public void SpSiteRepositoryCanGetRecordByUrlThrowsException()
        {
            ClientContext _ctx = GetValidContext();
            var _targetURL = "";
            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            SiteRequestInformation _siteInfo = _siteRequestManager.GetSiteRequestByUrl(_targetURL);
            Assert.IsNotNull(_siteInfo);
        }

        [TestMethod]
        public void SpSiteRepositoryDoesSiteExistShouldNotBeFound()
        {
            ClientContext _ctx = GetValidContext();
            bool _expectedResult = false;
            var _targetURL = "https://microsoftacs.onmicrosoft.com/YOURNOTGOINGTOFINDME";
            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            bool _actualResult = _siteRequestManager.DoesSiteRequestExist(_targetURL);
            Assert.AreEqual<bool>(_expectedResult, _actualResult);
        }

        [TestMethod]
        public void SpSiteRepositoryDoesSiteExistShoudBeFound()
        {
            ClientContext _ctx = GetValidContext();
            bool _expectedResult = true;
            var _targetURL = "https://microsoftacs.onmicrosoft.com/teams/test";
            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            bool _actualResult = _siteRequestManager.DoesSiteRequestExist(_targetURL);
            Assert.AreEqual<bool>(_expectedResult, _actualResult);
        }

        [TestMethod]
        public void SpSiteRepositoryUpdateRequestStatus()
        {
            ClientContext _ctx = GetValidContext();
            var _targetURL = "https://microsoftacs.onmicrosoft.com/teams/test";
            SiteRequestStatus _expectStatus = SiteRequestStatus.Processing;

            ISiteRequestFactory _targetFactory = SiteRequestFactory.GetInstance();
            ISiteRequestManager _siteRequestManager = _targetFactory.GetSPSiteRepository(_ctx, "SiteRequests");
            _siteRequestManager.UpdateRequestStatus(_targetURL, _expectStatus, "HI");
        
        }


        /// <summary>
        /// Used to create the Client Context
        /// </summary>
        /// <returns></returns>
        public ClientContext GetValidContext()
        {
            string _siteUrl = "https://microsoftacs.sharepoint.com/teams/frankdev";
            string _userName = "frank@microsoftacs.onmicrosoft.com";
            string _passWord = "@sharepoint";
            SecureString _secureStringPwd = new SecureString();
            foreach (char _c in _passWord)
                _secureStringPwd.AppendChar(_c);

            ClientContext _ctx = new ClientContext(_siteUrl);
            SharePointOnlineCredentials _creds = new SharePointOnlineCredentials(_userName, _secureStringPwd);
            _ctx.Credentials = _creds;
            _ctx.ApplicationName = "MSTEST-SITEPROVISIONING";
            _ctx.AuthenticationMode = ClientAuthenticationMode.Default;
            _ctx.ExecuteQuery();
            return _ctx;

        }

        public ClientContext GetInvalidContext()
        {
            string _siteUrl = "https://microsoftacs.sharepoint.com/teams/autositedev";
            string _userName = "INVALID@test.onmicrosoft.com";
            string _passWord = "INVALID";
            SecureString _secureStringPwd = new SecureString();
            foreach (char _c in _passWord)
                _secureStringPwd.AppendChar(_c);

            ClientContext _ctx = new ClientContext(_siteUrl);
            SharePointOnlineCredentials _creds = new SharePointOnlineCredentials(_userName, _secureStringPwd);
            _ctx.Credentials = _creds;
            _ctx.ApplicationName = "MSTEST-SITEPROVISIONING";
            _ctx.AuthenticationMode = ClientAuthenticationMode.Default;
            return _ctx;
        }
    }
}
