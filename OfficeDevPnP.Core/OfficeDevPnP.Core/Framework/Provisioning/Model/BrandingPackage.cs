using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    /// <summary>
    /// Domain Object that defines a Composed Look in the Site Template
    /// </summary>
    public partial class BrandingPackage
    {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string ColorFile { get; set; }
        [XmlAttribute]
        public string FontFile { get; set; }
        [XmlAttribute]
        public string BackgroundFile { get; set; }
        [XmlAttribute]
        public string MasterPage { get; set; }
        [XmlAttribute]
        public string SiteLogo { get; set; }
        [XmlAttribute]
        public string AlternateCSS { get; set; }
        [XmlAttribute]
        public int Version { get; set; }

    }
}
