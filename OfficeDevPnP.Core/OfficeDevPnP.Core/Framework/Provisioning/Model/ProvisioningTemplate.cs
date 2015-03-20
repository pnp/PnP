using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for the Provisioning Template
    /// </summary>
    [XmlRoot(ElementName = "SharePointProvisioningTemplate")]
    public class ProvisioningTemplate
    {
        #region private members

        private List<Field> _siteFields = new List<Field>();
        private List<ContentType> _contentTypes = new List<ContentType>();
        private List<PropertyBagEntry> _propertyBags = new List<PropertyBagEntry>();
        private List<ListInstance> _lists = new List<ListInstance>();
        private ComposedLook _composedLook = new ComposedLook();
        private Features _features = new Features();
        private CustomActions _customActions = new CustomActions();
        private List<File> _files = new List<File>();
        private List<Provider> _providers = new List<Provider>(); 
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
        public string SitePolicy { get; set; }

        public List<PropertyBagEntry> PropertyBagEntries
        {
            get { return this._propertyBags; }
            private set { this._propertyBags = value; }
        }
        
        /// <summary>
        /// Gets or Sets the Site Security
        /// </summary>
        public SiteSecurity Security { get; set; }

        /// <summary>
        /// Gets a collection of fields 
        /// </summary>
        public List<Field> SiteFields
        {
            get { return this._siteFields; }
            private set { this._siteFields = value; }
        }

        public List<ContentType> ContentTypes
        {
            get{ return this._contentTypes;}
            private set { this._contentTypes = value;}
        }

        public List<ListInstance> Lists
        {
            get { return this._lists; }
            private set { this._lists = value; }
        }

        public Features Features
        {
            get { return this._features; }
            set { this._features = value; }
        }

        public CustomActions CustomActions
        {
            get { return this._customActions; }
            set { this._customActions = value; }
        }

        public List<File> Files
        {
            get { return this._files; }
            private set { this._files = value; }
        }

        public ComposedLook ComposedLook
        {
            get { return this._composedLook; }
            set { this._composedLook = value; }
        }

        public List<Provider> Providers
        {
            get { return this._providers;  }
            private set { this._providers = value; }
        }

        #endregion
    }
}
