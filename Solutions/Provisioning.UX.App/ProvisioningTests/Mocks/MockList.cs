using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
 
namespace ProvisioningTests.Mocks
{
    [XmlRoot(ElementName = "List")]
    public class MockList
    {
        [XmlAttribute]
        public string Title { get; set; }

        [XmlAttribute]
        public string Description { get; set; }

        [XmlAttribute]
        public int BaseType { get; set; }

        [XmlAttribute]
        public bool EnableContentTypes { get; set; }

        [XmlAttribute]
        public bool VersioningEnbabled { get; set; }

        [XmlArray(ElementName = "ContentTypes")]
        [XmlArrayItem("ContentTypeRef", typeof(MockContentType))]
        public List<MockContentType> ContentTypes { get; set; }
    }
}
