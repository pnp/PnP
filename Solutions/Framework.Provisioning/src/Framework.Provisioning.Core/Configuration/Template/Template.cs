using Framework.Provisioning.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// Domain Object for the MasterTemplate
    /// </summary>
    [XmlRoot(ElementName = "Template")]
    public partial class Template
    {
        #region Private Members
        private List<ListInstance> _listInstances = new List<ListInstance>();
        #endregion

        #region Properties
        
        /// <summary>
        /// Gets the ID of the template
        /// </summary>
        [XmlAttribute]
        public string ID { get; set; }
       
        /// <summary>
        /// Gets the Name of the template
        /// </summary>
        [XmlAttribute]
        public string Name { get; set; }
       
        /// <summary>
        /// Gets the Title of the tempalte
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }
       
        /// <summary>
        /// Gets if the Temmplate is enabled
        /// </summary>
        [XmlAttribute]
        public bool Enabled { get; set; }
       
        /// <summary>
        /// Defines the SharePoint Site Template used by the custom site template
        /// </summary>
        [XmlAttribute]
        public string RootTemplate { get; set; }
       
        /// <summary>
        /// Gets the Description of the template
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
       
        /// <summary>
        /// Defines the Host path
        /// </summary>
        [XmlAttribute]
        public string HostPath { get; set; }

        /// <summary>
        /// Defines the Managed Path. Only Sites and Teams should be used
        /// </summary>
        [XmlAttribute]
        public string ManagedPath { get; set; }

        /// <summary>
        /// Gets the Verision of the custom template
        /// </summary>
        [XmlAttribute]
        public int Version { get; set; }

        /// <summary>
        /// Defines if the template is available on subsites
        /// </summary>
        [XmlAttribute] 
        public bool RootWebOnly { get; set; }

        /// <summary>
        /// Defines if the tempalte is on the subweb only
        /// </summary>
        [XmlAttribute]
        public bool SubWebOnly { get; set; }

        /// <summary>
        /// Defines the Composed look for the Site
        /// </summary>
        [XmlAttribute]
        public string BrandingPackage { get; set; }

        /// <summary>
        /// Gets or sets the storage quota of the new site.
        /// Not used in SharePoint On-premises builds
        /// </summary>
        [XmlAttribute]
        public long StorageMaximumLevel { get; set; }

        /// <summary>
        /// Gets or sets the amount of storage usage on the new site that triggers a warning.
        /// Not used in SharePoint On-premises builds
        /// </summary>
        [XmlAttribute]
        public long StorageWarningLevel { get; set; }

        /// <summary>
        /// Gets or sets the maximum amount of machine resources that can be used by user code on the new site.
        /// Not used in SharePoint On-premises builds
        /// </summary>
        [XmlAttribute]
        public long UserCodeMaximumLevel { get; set; }

        /// <summary>
        /// Gets or sets the amount of machine resources used by user code that triggers a warning.
        /// Not used in SharePoint On-premises builds
        /// </summary>
        [XmlAttribute]
        public long UserCodeWarningLevel { get; set; }

        /// <summary>
        /// Gets or Sets the Template Configuration
        /// </summary>
        [XmlAttribute]
        public string TemplateConfiguration { get; set; }

        /// <summary>
        /// Gets the Site Template
        /// Will return Null if the Template is not found in the Engine
        /// </summary>
        /// <returns></returns>
        public SiteTemplate GetSiteTemplate()
        {
            SiteTemplate _siteTemplate;

            if(!string.IsNullOrWhiteSpace(this.TemplateConfiguration))
            {
                string _assemblyPath = PathHelper.GetAssemblyDirectory();
                var _fullFilePath = System.IO.Path.Combine(_assemblyPath, this.TemplateConfiguration);
                bool _fileExist = System.IO.File.Exists(_fullFilePath);
 
                if(_fileExist) {
                    XDocument _doc = XDocument.Load(_fullFilePath);
                    _siteTemplate = XmlSerializerHelper.Deserialize<SiteTemplate>(_doc.Root.ToString());
                    _siteTemplate.SiteFields = this.GetSiteFields(_doc);
                    _siteTemplate.ContentTypes = this.GetContentTypes(_doc);

                    return _siteTemplate;
                }
                else
                {
                    Log.Warning("Framework.Provisioning.Core.Configuration.Template.GetSiteTemplate", "SiteTemplate configuration file {0} was not found.", _fullFilePath);
                    return null;
                }
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region Private Members
        private List<ContentType> GetContentTypes(XDocument doc)
        {
            List<ContentType> _contentTypes = new List<ContentType>();
            XName name = XName.Get("ContentTypes", doc.Root.GetDefaultNamespace().NamespaceName);
            XDocument _contentTypeDoc = new XDocument(doc.Descendants(name).First());
            XElement _element = _contentTypeDoc.Element("ContentTypes");

            foreach (var element in _element.Elements())
            {
                ContentType _field = new ContentType();
                _field.SchemaXml = element.ToString();
                _contentTypes.Add(_field);
            }

            return _contentTypes;
        }
        private List<Field> GetSiteFields(XDocument doc)
        {
            List<Field> _fields = new List<Field>();
            XName name = XName.Get("SiteFields", doc.Root.GetDefaultNamespace().NamespaceName);
            XDocument siteFieldsDoc = new XDocument(doc.Descendants(name).First());
            XElement fields = siteFieldsDoc.Element("SiteFields");
            foreach (var fieldElement in fields.Elements())
            {
                Field _field = new Field();
                _field.SchemaXml = fieldElement.ToString();
                _fields.Add(_field);
            }

            return _fields;
        }
        #endregion
    }
}
