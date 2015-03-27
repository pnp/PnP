using OfficeDevPnP.Core.Framework.Provisioning.Connectors;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for the Provisioning Template
    /// </summary>
    public class ProvisioningTemplate
    {
        #region private members
        private List<Field> _siteFields = new List<Field>();
        private List<ContentType> _contentTypes = new List<ContentType>();
        private List<PropertyBagEntry> _propertyBags = new List<PropertyBagEntry>();
        private List<ListInstance> _lists = new List<ListInstance>();
        private ComposedLook _composedLook = new ComposedLook();
        private Features _features = new Features();
        private SiteSecurity _siteSecurity = new SiteSecurity();
        private CustomActions _customActions = new CustomActions();
        private List<File> _files = new List<File>();
        private List<Provider> _providers = new List<Provider>();
        private FileConnectorBase connector;
        #endregion

        #region Constructor
        public ProvisioningTemplate()
        {
            this.connector = new FileSystemConnector(".", "");
        }

        public ProvisioningTemplate(FileConnectorBase connector)
        {
            this.connector = connector;
        }
        #endregion


        #region Properties
        /// <summary>
        /// Gets or sets the ID of the Provisioning Template
        /// </summary>
        public string ID { get; set; }

        /// <summary>
        /// Gets or sets the Version of the Provisioning Template
        /// </summary>
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
        /// Security Groups Members for the Template
        /// </summary>
        public SiteSecurity Security
        {
            get { return this._siteSecurity; }
            set { this._siteSecurity = value; }
        }

        /// <summary>
        /// Gets a collection of fields 
        /// </summary>
        public List<Field> SiteFields
        {
            get { return this._siteFields; }
            private set { this._siteFields = value; }
        }

        /// <summary>
        /// Gets a collection of Content Types to create
        /// </summary>
        public List<ContentType> ContentTypes
        {
            get { return this._contentTypes; }
            private set { this._contentTypes = value; }
        }

        public List<ListInstance> Lists
        {
            get { return this._lists; }
            private set { this._lists = value; }
        }

        /// <summary>
        /// Gets or sets a list of features to activate or deactivate
        /// </summary>
        public Features Features
        {
            get { return this._features; }
            set { this._features = value; }
        }

        /// <summary>
        /// Gets or sets CustomActions for the template
        /// </summary>
        public CustomActions CustomActions
        {
            get { return this._customActions; }
            set { this._customActions = value; }
        }

        /// <summary>
        /// Gets a collection of files for the template
        /// </summary>
        public List<File> Files
        {
            get { return this._files; }
            private set { this._files = value; }
        }

        /// <summary>
        /// Gets or Sets the composed look of the template
        /// </summary>
        public ComposedLook ComposedLook
        {
            get { return this._composedLook; }
            set { this._composedLook = value; }
        }

        /// <summary>
        /// Gets a collection of Providers that are used during the extensibility pipeline
        /// </summary>
        public List<Provider> Providers
        {
            get { return this._providers; }
            private set { this._providers = value; }
        }

        public FileConnectorBase Connector
        {
            get
            {
                return this.connector;
            }
            set
            {
                this.connector = value;
            }
        }

        #endregion
    }
}
