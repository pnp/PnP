using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Xml.Serialization;

namespace Provisioning.SubsiteAndThemeWeb {
    [XmlRoot(ElementName="SiteConfig")]
    public class ProvisioningConfiguration {
        [XmlArray(ElementName = "AllowedTemplates")]
        [XmlArrayItem(ElementName = "Template")]
        public Template[] Templates { get; set; }

        public Branding Branding { get; set; }
    }

    public class Template {
        [XmlAttribute]
        public string DisplayName { get; set; }
        [XmlAttribute]
        public string TemplateId { get; set; }
    }

    public class Branding {
        [XmlAttribute]
        public string LogoUrl { get; set; }

        [XmlAttribute]
        public string LogoFilePath { get; set; }

        [XmlElement(ElementName = "Theme")]
        public Theme[] Themes { get; set; }
    }

    public class Theme {
        [XmlAttribute]
        public string Name { get; set; }
        [XmlAttribute]
        public string MasterPageUrl { get; set; }
        [XmlAttribute]
        public string FontFile { get; set; }
        [XmlAttribute]
        public string ColorFile { get; set; }
        [XmlAttribute]
        public string BackgroundFile { get; set; }
    }
}