using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using OfficeDevPnP.Core.Framework.Provisioning.Model;
using System.Xml.Linq;
using OfficeDevPnP.Core.Utilities;
using System.IO;
using System.Linq;
using Microsoft.SharePoint.Client;
using OfficeDevPnP.Core.Framework.Provisioning.Providers.Xml;
using OfficeDevPnP.Core.Framework.Provisioning.Providers;

namespace OfficeDevPnP.Core.Tests.Framework.ProvisioningTemplates
{
    [TestClass]
    public class DomainModelTests
    {
        private string _provisioningTemplatePath1 = string.Empty;
        private string _provisioningTemplatePath1NamespaceURI = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_03;
        private string _provisioningTemplatePath2 = string.Empty;
        private string _provisioningTemplatePath2NamespaceURI = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_03;
        private string _provisioningTemplatePath5 = string.Empty;
        private string _provisioningTemplatePath5NamespaceURI = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05;
        private string _provisioningTemplatePath6 = string.Empty;
        private string _provisioningTemplatePath6NamespaceURI = XMLConstants.PROVISIONING_SCHEMA_NAMESPACE_2015_05;
        private const string TEST_CATEGORY = "Framework Provisioning Domain Model";

        [TestInitialize()]
        public void Initialize()
        {
            this._provisioningTemplatePath1 = string.Format(@"{0}\..\..\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningTemplate-2015-03-Sample-01.xml");
            this._provisioningTemplatePath2 = string.Format(@"{0}\..\..\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningTemplate-2015-03-Sample-02.xml");
            this._provisioningTemplatePath5 = string.Format(@"{0}\..\..\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningSchema-2015-05-FullSample-01.xml");
            this._provisioningTemplatePath6 = string.Format(@"{0}\..\..\Resources\Templates\{1}", AppDomain.CurrentDomain.BaseDirectory, "ProvisioningSchema-2015-05-ReferenceSample-01.xml");
        }

        #region Formatter Tests

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanDeserializeXMLToDomainObject1()
        {
            this.GetProvisioningTemplate();
            
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath1, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);
                Assert.IsNotNull(_pt);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXML1()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath1, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsTrue(formatter.IsValid(_formattedTemplateBack));
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXMLStream1()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath1, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsNotNull(_formattedTemplateBack);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanDeserializeXMLToDomainObject2()
        {
            this.GetProvisioningTemplate();

            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath2, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);
                Assert.IsNotNull(_pt);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXML2()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath2, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsTrue(formatter.IsValid(_formattedTemplateBack));
            }

        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetTemplateNameandVersion()
        {
            var _expectedID = "SPECIALTEAM";
            var _expectedVersion = 1.0;

            var _pt = this.GetProvisioningTemplate();
            Assert.AreEqual(_expectedID, _pt.Id);
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
        public void CanGetAdministrators()
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

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetFeatures()
        {
            var _pt = this.GetProvisioningTemplate();
            var _sfs = _pt.Features.SiteFeatures;

            var _expectedSiteFeaturesCount = 3;
            var _expectedWebFeaturesCount = 4;

            Assert.AreEqual(_expectedSiteFeaturesCount, _pt.Features.SiteFeatures.Count);
            Assert.AreEqual(_expectedWebFeaturesCount, _pt.Features.WebFeatures.Count);

            foreach(var _f in _sfs)
            {
                Assert.IsTrue(_f.Id != Guid.Empty);
            }

            var f = new OfficeDevPnP.Core.Framework.Provisioning.Model.Feature();
            _pt.Features.SiteFeatures.Add(f);

            
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanGetCustomActions()
        {
            var _pt = this.GetProvisioningTemplate();
            var _csa = _pt.CustomActions.SiteCustomActions.FirstOrDefault();
            Assert.IsNotNull(_csa.Rights);
        }
     
        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeToJSon()
        {
           var _pt = this.GetProvisioningTemplate();
           var _json = JsonUtility.Serialize<ProvisioningTemplate>(_pt);
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeToXml()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var _pt = ctx.Web.GetProvisioningTemplate();

                ITemplateFormatter formatter = XMLPnPSchemaFormatter.LatestFormatter;
                var _formattedTemplate = formatter.ToFormattedTemplate(_pt);

                Assert.IsNotNull(_formattedTemplate);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void ValidateFullProvisioningSchema5()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath5, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath5NamespaceURI);
                Boolean isValid = formatter.IsValid(_formattedTemplate);

                Assert.IsTrue(isValid);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void ValidateSharePointProvisioningSchema6()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath6, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath6NamespaceURI);
                Boolean isValid = formatter.IsValid(_formattedTemplate);

                Assert.IsTrue(isValid);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanDeserializeXMLToDomainObject5()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath5, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath5NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);
                Assert.IsNotNull(_pt);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanDeserializeXMLToDomainObject6()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath6, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath6NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);
                Assert.IsNotNull(_pt);
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXML6()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath6, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath6NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate);

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsTrue(formatter.IsValid(_formattedTemplateBack));
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXML5ByIdentifier()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath5, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath5NamespaceURI);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate, "SPECIALTEAM");

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsTrue(formatter.IsValid(_formattedTemplateBack));
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void CanSerializeDomainObjectToXML5ByFileLink()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath5, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath5NamespaceURI);

                XMLTemplateProvider provider =
                    new XMLFileSystemTemplateProvider(
                        String.Format(@"{0}\..\..\Resources",
                        AppDomain.CurrentDomain.BaseDirectory),
                        "Templates");

                formatter.Initialize(provider);
                var _pt = formatter.ToProvisioningTemplate(_formattedTemplate, "WORKFLOWSITE");

                var _formattedTemplateBack = formatter.ToFormattedTemplate(_pt);

                Assert.IsTrue(formatter.IsValid(_formattedTemplateBack));
            }
        }

        #endregion

        #region Comparison Tests

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void AreTemplatesEqual()
        {
            ProvisioningTemplate _pt1 = null;
            ProvisioningTemplate _pt2 = null;

            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath1, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                _pt1 = formatter.ToProvisioningTemplate(_formattedTemplate);
            }

            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath2, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                _pt2 = formatter.ToProvisioningTemplate(_formattedTemplate);
            } 

            Assert.IsFalse(_pt1.Equals(_pt2));
            Assert.IsTrue(_pt1.Equals(_pt1));
        }

        #endregion

        #region Test Support
        /// <summary>
        /// Test Support to return ProvisionTemplate 
        /// </summary>
        /// <returns></returns>
        protected ProvisioningTemplate GetProvisioningTemplate()
        {
            using (Stream _formattedTemplate = new FileStream(this._provisioningTemplatePath1, FileMode.Open, FileAccess.Read, FileShare.Read))
            {
                ITemplateFormatter formatter = XMLPnPSchemaFormatter.GetSpecificFormatter(this._provisioningTemplatePath1NamespaceURI);
                return (formatter.ToProvisioningTemplate(_formattedTemplate));
            }
        }

        [TestMethod]
        [TestCategory(TEST_CATEGORY)]
        public void GetRemoteTemplateTest()
        {
            using (var ctx = TestCommon.CreateClientContext())
            {
                var template = ctx.Web.GetProvisioningTemplate();
                Assert.IsTrue(template.Lists.Any());
            }
        }

        #endregion
    }
}
