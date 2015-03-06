using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    [XmlRoot(ElementName = "SiteTemplate")]
    public class SiteTemplate
    {
        #region Private Members
        private List<Features> _features = new List<Features>();
        private List<Field> _siteFields = new List<Field>();
        private List<ContentType> _contentTypes = new List<ContentType>();
        private List<ListInstance> _listInstances = new List<ListInstance>();
        private List<CustomAction> _customActions = new List<CustomAction>();
        private List<Provider> _providers = new List<Provider>();
        private List<File> _files = new List<File>();
        #endregion

        #region Public Members
        [XmlElement]
        public string DefaultSitePolicy { get; set; }
       
        /// <summary>
        /// Defines Site Security
        /// </summary>
        [XmlElement]
        public SiteSecurity Security { get; set; }
        
        /// <summary>
        /// Features defined in the site template
        /// </summary>
        [XmlElement]
        public Features Features
        {
            get;
            set;
        }

        /// <summary>
        /// Fields to Provision
        /// </summary>
        public List<Field> SiteFields
        {
            get { return this._siteFields; }
            set { this._siteFields = value; }
        }

        /// <summary>
        /// Content Types
        /// </summary>
        public List<ContentType> ContentTypes
        {
            get { return this._contentTypes; }
            set { this._contentTypes = value; }
        }
       
        /// <summary>
        /// A Collection of Lists to Create
        /// </summary>
        [XmlArray(ElementName = "Lists")]
        [XmlArrayItem("ListInstance", typeof(ListInstance))]
        public List<ListInstance> ListInstances
        {
            get { return this._listInstances; }
            set { this._listInstances = value; }
        }
        [XmlElement]
        public CustomActions CustomActions
        {
            get;
            set;
        }
        ///// <summary>
        ///// Defines custom actions for the Site
        ///// </summary>
        //[XmlArray(ElementName = "CustomActions")]
        //[XmlArrayItem("CustomAction", typeof(CustomAction))]
        //public List<CustomAction> CustomActions1
        //{
        //    get { return this._customActions;}
        //    set { this._customActions = value; }
        //}

        /// <summary>
        /// Extensibly Providers
        /// </summary>
        [XmlArray(ElementName = "Providers")]
        [XmlArrayItem("Provider", typeof(Provider))]
        public List<Provider> Providers
        {
            get { return this._providers; }
            set { this._providers = value; }
        }

        /// <summary>
        /// Files to deploy 
        /// </summary>
        [XmlArray(ElementName = "Files")]
        [XmlArrayItem("File", typeof(File))]
        public List<File> Files
        {
            get { return this._files; }
            set { this._files = value; }
        }
        #endregion
    }
}
