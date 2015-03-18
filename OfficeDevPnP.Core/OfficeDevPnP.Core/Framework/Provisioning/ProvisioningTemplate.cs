using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    [XmlRoot(ElementName = "SharePointProvisioningTemplate")]
    [Serializable]
    public class ProvisioningTemplate
    {
        #region private members
        private List<Field> _siteFields = new List<Field>();
        #endregion

        #region Properties
        [XmlAttribute]
        public string ID { get; set; }

        [XmlAttribute]
        public double Version { get; set; }

        [XmlElement]
        public string DefaultSitePolicy { get; set; }

        /// <summary>
        /// Fields to Provision
        /// </summary>
        [XmlArray(ElementName = "SiteFields")]
        [XmlArrayItem("Field", typeof(Field))]
        public List<Field> SiteFields
        {
            get { return this._siteFields; }
            set { this._siteFields = value; }
        }
        #endregion
    }
}
