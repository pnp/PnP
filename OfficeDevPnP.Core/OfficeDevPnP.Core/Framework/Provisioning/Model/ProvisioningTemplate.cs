using System;
using System.Collections.Generic;
using System.Linq;
using OfficeDevPnP.Core.Extensions;
using OfficeDevPnP.Core.Framework.Provisioning.Connectors;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for the Provisioning Template
    /// </summary>
    public class ProvisioningTemplate : IEquatable<ProvisioningTemplate>
    {
        #region private members
        private Dictionary<string,string> _parameters = new Dictionary<string, string>(); 
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
        private List<Page> _pages = new List<Page>(); 
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
        /// Any parameters that can be used throughout the template
        /// </summary>
        public Dictionary<string, string> Parameters
        {
            get { return _parameters; }
            private set { _parameters = value; }
        }
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

        public List<Page> Pages
        {
            get { return this._pages; }
            private set { this._pages = value; }
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

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}|{3}|{4}|{5}|{6}|{7}|{8}|{9}|{10}|{11}|{12}|{13}|{14}|{15}|{16}|{17}",
                this.ComposedLook.GetHashCode(),
                this.ContentTypes.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.CustomActions.SiteCustomActions.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.CustomActions.WebCustomActions.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Features.SiteFeatures.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Features.WebFeatures.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Files.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.ID,
                this.Lists.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.PropertyBagEntries.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Providers.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Security.AdditionalAdministrators.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Security.AdditionalMembers.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Security.AdditionalOwners.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.Security.AdditionalVisitors.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.SiteFields.Aggregate(0, (acc, next) => acc += next.GetHashCode()),
                this.SitePolicy,
                this.Version
                ).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ProvisioningTemplate))
            {
                return (false);
            }
            return (Equals((ProvisioningTemplate)obj));
        }

        public bool Equals(ProvisioningTemplate other)
        {
            return (
                this.ComposedLook == other.ComposedLook &&
                this.ContentTypes.DeepEquals(other.ContentTypes) &&
                this.CustomActions.SiteCustomActions.DeepEquals(other.CustomActions.SiteCustomActions) &&
                this.CustomActions.WebCustomActions.DeepEquals(other.CustomActions.WebCustomActions) &&
                this.Features.SiteFeatures.DeepEquals(other.Features.SiteFeatures) &&
                this.Features.WebFeatures.DeepEquals(other.Features.WebFeatures) &&
                this.Files.DeepEquals(other.Files) &&
                this.ID == other.ID &&
                this.Lists.DeepEquals(other.Lists) &&
                this.PropertyBagEntries.DeepEquals(other.PropertyBagEntries) &&
                this.Providers.DeepEquals(other.Providers) &&
                this.Security.AdditionalAdministrators.DeepEquals(other.Security.AdditionalAdministrators) &&
                this.Security.AdditionalMembers.DeepEquals(other.Security.AdditionalMembers) &&
                this.Security.AdditionalOwners.DeepEquals(other.Security.AdditionalOwners) &&
                this.Security.AdditionalVisitors.DeepEquals(other.Security.AdditionalVisitors) &&
                this.SiteFields.DeepEquals(other.SiteFields) &&
                this.SitePolicy == other.SitePolicy &&
                this.Version == other.Version);
        }

        #endregion
    }
}
