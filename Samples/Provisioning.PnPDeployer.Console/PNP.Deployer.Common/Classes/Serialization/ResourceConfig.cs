using System;
using System.Xml.Serialization;

namespace PNP.Deployer.Common
{
    [Serializable()]
    public class ResourceConfig
    {
        #region Attributes

        [XmlAttribute("LCID")]
        public int LCID { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }

        #endregion
    }
}
