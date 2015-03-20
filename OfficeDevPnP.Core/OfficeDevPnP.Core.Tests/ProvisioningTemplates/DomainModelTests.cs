using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System.Xml.Linq;
using OfficeDevPnP.Core.Utilities;
using System.IO;

namespace OfficeDevPnP.Core.Tests.ProvisioningTemplates
{
    [TestClass]
    public class DomainModelTests
    {
        private string _provisioningTemplatePath = string.Empty;
        private const string TEST_CATEGORY = "Provisioning Template Domain Model";

        [TestInitialize()]
        public void Intialize()
        {
            this._provisioningTemplatePath = string.Format(@"{0}\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningTemplate.xml");
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanDeserializeXMLToDomainObject()
        {
            this.GetProvisioningTemplate();
            XDocument _doc = XDocument.Load(this._provisioningTemplatePath);
            var _pt = XMLSerializer.Deserialize<ProvisioningTemplate>(_doc);
            Assert.IsNotNull(_pt);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetTemplateNameandVersion()
        {
            var _expectedID = "SPECIALTEAM";
            var _expectedVersion = 1.0;

            var _pt = this.GetProvisioningTemplate();
            Assert.AreEqual(_expectedID, _pt.ID);
            Assert.AreEqual(_expectedVersion, _pt.Version);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetPropertyBagEntries()
        {
            var _expectedCount = 2;
            var _pt = GetProvisioningTemplate();

            var _pb1KEY = "KEY1";
            var _pb1Value = "value1";
            var _pb2KEY = "KEY2";
            var _pb2Value = "value2";

            Assert.AreEqual(_expectedCount, _pt.PropertyBagEntries.Count);

            var _pb1 = _pt.PropertyBagEntries[0];
            Assert.AreEqual(_pb1KEY, _pb1.Key);
            Assert.AreEqual(_pb1Value, _pb1.Value);

            var _pb2 = _pt.PropertyBagEntries[1];
            Assert.AreEqual(_pb2KEY, _pb2.Key);
            Assert.AreEqual(_pb2Value, _pb2.Value);
        }
     
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetOwners()
        {
            var _pt = GetProvisioningTemplate();
            var _expectedCount = 2;
            var _expectedUser1 = "user@contoso.com";
            var _expectedUser2 = "U_SHAREPOINT_ADMINS";

            var _siteSecurity = _pt.Security;
            var _users = _pt.Security.AdditionalOwners;
            Assert.AreEqual(_expectedCount, _users.Count);

            var _u1 = _pt.Security.AdditionalOwners[0].Name;
            var _u2 = _pt.Security.AdditionalOwners[1].Name;

            Assert.AreEqual(_expectedUser1, _u1);
            Assert.AreEqual(_expectedUser2, _u2);
        }
        
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetAdminstrators()
        {
            var _pt = GetProvisioningTemplate();
            var _expectedCount = 2;
            var _expectedUser1 = "user@contoso.com";
            var _expectedUser2 = "U_SHAREPOINT_ADMINS";

            var _siteSecurity = _pt.Security;
            var _users = _pt.Security.AdditionalAdministrators;
            Assert.AreEqual(_expectedCount, _users.Count);

            var _u1 = _pt.Security.AdditionalAdministrators[0].Name;
            var _u2 = _pt.Security.AdditionalAdministrators[1].Name;

            Assert.AreEqual(_expectedUser1, _u1);
            Assert.AreEqual(_expectedUser2, _u2);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetMembers()
        {
            var _pt = GetProvisioningTemplate();
            var _expectedCount = 2;
            var _expectedUser1 = "user@contoso.com";
            var _expectedUser2 = "U_SHAREPOINT_ADMINS";

            var _siteSecurity = _pt.Security;
            var _users = _pt.Security.AdditionalMembers;
            Assert.AreEqual(_expectedCount, _users.Count);

            var _u1 = _pt.Security.AdditionalMembers[0].Name;
            var _u2 = _pt.Security.AdditionalMembers[1].Name;

            Assert.AreEqual(_expectedUser1, _u1);
            Assert.AreEqual(_expectedUser2, _u2);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetVistors()
        {
            var _pt = GetProvisioningTemplate();
            var _expectedCount = 2;
            var _expectedUser1 = "user@contoso.com";
            var _expectedUser2 = "U_SHAREPOINT_ADMINS";

            var _siteSecurity = _pt.Security;
            var _additionalAdmins = _pt.Security.AdditionalVisitors;
            Assert.AreEqual(_expectedCount, _additionalAdmins.Count);

            var _u1 = _pt.Security.AdditionalVisitors[0].Name;
            var _u2 = _pt.Security.AdditionalVisitors[1].Name;

            Assert.AreEqual(_expectedUser1, _u1);
            Assert.AreEqual(_expectedUser2, _u2);
        }

        #region Test Support
        /// <summary>
        /// Test Support to return ProvisionTemplate 
        /// </summary>
        /// <returns></returns>
        protected ProvisioningTemplate GetProvisioningTemplate()
        {
            XDocument _doc = XDocument.Load(this._provisioningTemplatePath);
            return XMLSerializer.Deserialize<ProvisioningTemplate>(_doc);
        }
        #endregion
    }
}
