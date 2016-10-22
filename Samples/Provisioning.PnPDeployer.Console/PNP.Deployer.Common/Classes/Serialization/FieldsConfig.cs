using System;
using System.Xml.Serialization;
using System.Collections.Generic;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer.Common
{
    [Serializable()]
    [XmlRoot(ElementName = "ProviderConfiguration", Namespace = "http://PNP.Deployer/ProviderConfiguration")]
    public class FieldsConfig
    {
        #region Children

        [XmlArray("Fields")]
        [XmlArrayItem("Field", typeof(FieldConfig))]
        public List<FieldConfig> Fields { get; set; }

        #endregion
    }
}
