using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Framework.Provisioning.Core;
using Framework.Provisioning.Core.Authentication;
using System.Collections.Generic;

namespace ProvisioningTests._5_ProfileService
{
    [TestClass]
    public class ProfileServiceTests
    {
        const string _onpremaccountName = @"marascohome\brianmic";
        const string _office365Account = "i:0#.f|membership|frank@marascohome.com";

        [TestMethod]
        [TestCategory("Profile Service")]
        public void GetAllPropertiesForUser()
        {
            var NOTEXPECTEDCOUNT = 0;

            ProfileService _ps = new ProfileService();
            var auth = new AppOnlyAuthenticationTenant();
            _ps.Authentication = auth;
            var _props = _ps.GetAllPropertiesForUser(_onpremaccountName);
            Assert.AreNotEqual(NOTEXPECTEDCOUNT, _props.Count);
         
        }

        [TestMethod]
        [TestCategory("Profile Service")]
        public void GetSpecificPropertiesForUser()
        {
            var NOTEXPECTEDCOUNT = 0;

            ProfileService _ps = new ProfileService();
             var auth = new AppOnlyAuthenticationTenant();
             _ps.Authentication = new AppOnlyAuthenticationTenant();
       
            var propstoGet = new string[] { "PreferredName", "WorkPhone", "Title" };
            var _props = _ps.GetPropertiesForUser(_onpremaccountName, propstoGet);

            var _items = _props as IList<string>; 
            Assert.AreNotEqual(NOTEXPECTEDCOUNT, _items.Count);
        }

        [TestMethod]
        [TestCategory("Profile Service")]
        public void UpdateUserPropertySingleValue()
        {
            ProfileService _ps = new ProfileService();
            var _auth = new TenantAccountAuthentication();
            _ps.Authentication = _auth;
            _ps.SetUserPropertySingleValue(_onpremaccountName, "AboutMe", "CSOM UPDATE");

        }

        [TestMethod]
        [TestCategory("Profile Service")]
        public void UpdateUserPropertyMultiValue()
        {
            ProfileService _ps = new ProfileService();
            TenantAccountAuthentication _auth = new TenantAccountAuthentication();
            _ps.Authentication = _auth;

            // List Multiple values
            List<string> skills = new List<string>() { "SharePoint", "Office 365", "C#", "Java" };
            _ps.SetUserProfilePropertyMultiValue(_onpremaccountName, "SPS-Skills", skills);
        }

    }
}
