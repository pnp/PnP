using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    /// <summary>
    /// Domain Object for the Provisioning Template
    /// </summary>
    [XmlRoot(ElementName = "SharePointProvisioningTemplate")]
    [Serializable]
    public class ProvisioningTemplate
    {
        #region private members
        private List<Field> _siteFields = new List<Field>();
        private List<PropertyBagEntry> _propertyBags = new List<PropertyBagEntry>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the ID of the Provisioning Template
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Version of the Provisioning Template
        /// </summary>
        [XmlAttribute]
        public double Version { get; set; }

        /// <summary>
        /// Gets or Sets the Site Policy
        /// </summary>
        [XmlElement]
        public string SitePolicy { get; set; }

        [XmlArray(ElementName = "PropertyBagEntries")]
        [XmlArrayItem("PropertyBagEntry", typeof(PropertyBagEntry))]
        public List<PropertyBagEntry> PropertyBagEntries
        {
            get { return this._propertyBags; }
            set { this._propertyBags = value; }
        }
        /// <summary>
        /// Gets or Sets the Site Security
        /// </summary>
        [XmlElement]
        public SiteSecurity Security { get; set; }

        /// <summary>
        /// Gets a collection of fields 
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
