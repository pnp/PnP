using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for List Creation
    /// </summary>
    public class ListInstance
    {
        private List<ContentTypeBinding> _ctBindings = new List<ContentTypeBinding>();
        
        /// <summary>
        /// The Title of the list
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }
        
        /// <summary>
        /// The Description of the list
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
        
        /// <summary>
        /// DocumentTemplate
        /// </summary>
        [XmlAttribute]
        public string DocumentTemplate { get; set; }
        
        /// <summary>
        /// Add to QuickLaunch
        /// </summary>
        [XmlAttribute]
        public bool OnQuickLaunch { get; set; }
        
        /// <summary>
        /// The Template Type
        /// https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.listtemplatetype.aspx
        /// </summary>
        [XmlAttribute]
        public int TemplateType { get; set; }
        
        /// <summary>
        /// The Url Of list
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }
       
        /// <summary>
        /// Enable Versioning
        /// </summary>
        [XmlAttribute]
        public bool EnableVersioning { get; set; }

        /// <summary>
        /// Removes the Default Content Type from the library
        /// </summary>
        [XmlAttribute]
        public bool RemoveDefaultContentType { get; set; }
  
        /// <summary>
        /// Domain Object for Content Type Bindings
        /// </summary>
        [XmlArray(ElementName = "ContentTypeBindings")]
        [XmlArrayItem("ContentTypeBinding", typeof(ContentTypeBinding))]
        public List<ContentTypeBinding> GetContentTypeBindings
        {
            get { return this._ctBindings; }
            set { this._ctBindings = value;}
        }
    }
}
