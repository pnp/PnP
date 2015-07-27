using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ProvisioningTests.Mocks;
using System.Collections.Generic;
using Provisioning.Common.Utilities;

namespace ProvisioningTests._2_Utilities
{
    [TestClass]
    public class XMLSerializerTests
    {
        const string XMLSTRING_MOCK = "<?xml version=\"1.0\" encoding=\"utf-16\"?>\r\n<List xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xmlns:xsd=\"http://www.w3.org/2001/XMLSchema\" Title=\"ListTitle\" Description=\"Description\" BaseType=\"1\" EnableContentTypes=\"true\" VersioningEnbabled=\"false\">\r\n  <ContentTypes>\r\n    <ContentTypeRef ID=\"0x0101000728167cd9c94899925ba69c4af6743e01\" Name=\"SAMPLE1 CONTENT TYPE\" IsDefault=\"false\" />\r\n    <ContentTypeRef ID=\"0x0101000728167cd9c94899925ba69c4af6743e02\" Name=\"SAMPLE1 CONTENT TYPE\" IsDefault=\"false\" />\r\n  </ContentTypes>\r\n</List>";
     
        [TestMethod]
        [TestCategory("Utilities XmlSerializerHelper")]
        public void XMLSerializerCanSerializeObjectToXML()
        {
            var _listMockOjbect = this.CreateMockObject();
            var _xmlStringActual = XmlSerializerManager.Serialize<MockList>(_listMockOjbect);
           
            Assert.AreNotEqual(string.Empty, _xmlStringActual);
            Assert.AreEqual(XMLSTRING_MOCK, _xmlStringActual);
        }

        [TestMethod]
        [TestCategory("Utilities XmlSerializerHelper")]
        public void XMLSerializerCanDeserializationObject()
        {
            var _expectedObject = this.CreateMockObject();
            var _mockObjectActual = XmlSerializerManager.Deserialize<MockList>(XMLSTRING_MOCK);

            Assert.AreEqual(_expectedObject.Title, _mockObjectActual.Title);
            Assert.AreEqual(_expectedObject.Description, _mockObjectActual.Description);
            Assert.AreEqual(_expectedObject.EnableContentTypes, _mockObjectActual.EnableContentTypes);
        }

        public MockList CreateMockObject()
        {
            var _listMockObject = new MockList()
            {
                Title = "ListTitle",
                Description = "Description",
                BaseType = 1,
                EnableContentTypes = true,
                VersioningEnbabled = false
            };

            List<MockContentType> _contentTypeRefs = new List<MockContentType>();

            MockContentType _cti1 = new MockContentType()
            {
                ID = "0x0101000728167cd9c94899925ba69c4af6743e01",
                Name = "SAMPLE1 CONTENT TYPE"
            };

            MockContentType _cti2 = new MockContentType()
            {
                ID = "0x0101000728167cd9c94899925ba69c4af6743e02",
                Name = "SAMPLE1 CONTENT TYPE"
            };

            _contentTypeRefs.Add(_cti1);
            _contentTypeRefs.Add(_cti2);

            _listMockObject.ContentTypes = _contentTypeRefs;

            return _listMockObject;
        }
    }

}
