using System;
using System.Xml.Serialization;
using System.Collections.Generic;


// =======================================================
/// <author>
/// Simon-Pierre Plante (sp.plante@gmail.com)
/// </author>
// =======================================================
namespace PNP.Deployer.ExtensibilityProviders.SSOM
{
    #region SiteFieldsConfigurationData

    [Serializable()]
    [XmlRoot(ElementName = "ProviderConfiguration", Namespace = "http://PNP.Deployer/ProviderConfiguration")]
    public class SiteFieldsConfigurationData
    {
        [XmlArray("Fields")]
        [XmlArrayItem("Field", typeof(Field))]
        public List<Field> Fields { get; set; }
    }

    #endregion


    #region Field

    [Serializable()]
    public class Field
    {
        [XmlAttribute("Name")]
        public string Name { get; set; }

        [XmlArray("TitleResources")]
        [XmlArrayItem("TitleResource", typeof(TitleResource))]
        public List<TitleResource> TitleResources { get; set; }
    }

    #endregion


    #region TitleResource

    [Serializable()]
    public class TitleResource
    {
        [XmlAttribute("LCID")]
        public int LCID { get; set; }

        [XmlAttribute("Value")]
        public string Value { get; set; }
    }

    #endregion
}
