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

        [XmlIgnore]
        public int? Version { get; set; }

        [XmlAttribute("Version")]
        public object VersionValue
        {
            get { return this.Version; }
            set
            {
                if (value == null)
                {
                    this.Version = null;
                }
                else if (value is int || value is int?)
                {
                    this.Version = (int)value;
                }
                else
                {
                    this.Version = int.Parse(value.ToString());
                }
            }
        }
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
