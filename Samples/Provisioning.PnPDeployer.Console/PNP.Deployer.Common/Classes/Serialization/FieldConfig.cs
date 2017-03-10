using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace PNP.Deployer.Common
{
    [Serializable()]
    public class FieldConfig
    {
        #region Attributes

        [Required]
        [XmlAttribute("Name")]
        public string Name { get; set; }

        #endregion


        #region Children

        [XmlArray("TitleResources")]
        [XmlArrayItem("TitleResource", typeof(ResourceConfig))]
        public List<ResourceConfig> TitleResources { get; set; }

        #endregion
    }
}
