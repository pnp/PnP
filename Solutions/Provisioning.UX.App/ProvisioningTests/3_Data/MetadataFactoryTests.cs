using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Provisioning.Common.Data.Metadata;
using System.IO;
using Newtonsoft.Json;
using Provisioning.Common;
using System.Collections.Generic;

namespace ProvisioningTests._3_Data
{
    [TestClass]
    public class MetadataFactoryTests
    {
        [TestMethod]
        [TestCategory("MetadataFactoryTests")]
        public void CanGetInstanceManager()
        {
            IMetadataFactory _factory = MetadataFactory.GetInstance();
            IMetadataManager _manager = _factory.GetManager();

            Assert.IsInstanceOfType(_manager, typeof(IMetadataManager));
            Assert.IsNotNull(_manager);
        }
        [TestMethod]
        [TestCategory("MetadataFactoryTests")]
        public void CanCreateNewSiteClassification()
        {
            IMetadataFactory _factory = MetadataFactory.GetInstance();
            IMetadataManager _manager = _factory.GetManager();
            var _siteClassificationMock = GetMock();
            _manager.CreateNewSiteClassification(_siteClassificationMock);
        }

        [TestMethod]
        [TestCategory("MetadataFactoryTests")]
        public void CanGetSiteClassificationByName()
        {
            IMetadataFactory _factory = MetadataFactory.GetInstance();
            IMetadataManager _manager = _factory.GetManager();

            var _expectedMock = GetMock();
            var _actualMock = _manager.GetSiteClassificationByName(_expectedMock.Key);

            Assert.AreEqual(_expectedMock.Key, _actualMock.Key);
        }

        [TestMethod]
        [TestCategory("MetadataFactoryTests")]
        public void CanGetEnabledSiteClassifications()
        {
            IMetadataFactory _factory = MetadataFactory.GetInstance();
            IMetadataManager _manager = _factory.GetManager();
            var _actual = _manager.GetAvailableSiteClassifications();
        }

        private SiteClassification GetMock()
        {
            var _siteClassificationMock = new SiteClassification();
            _siteClassificationMock.AddAllAuthenticatedUsers = true;
            _siteClassificationMock.DisplayOrder = 1;
            _siteClassificationMock.Value = "TEST";
            _siteClassificationMock.Key = "TEST";
            _siteClassificationMock.Enabled = true;

            return _siteClassificationMock;
        }

    }
}
