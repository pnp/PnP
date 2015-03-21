using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that specifies the properties of the new list.
    /// </summary>
    public class ListInstance
    {
        #region Private Members
        private List<ContentTypeBinding> _ctBindings = new List<ContentTypeBinding>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list title
        /// </summary>
        [XmlAttribute]
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the list
        /// </summary>
        [XmlAttribute]
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies the identifier of the document template for the new list.
        /// </summary>
        [XmlAttribute]
        public string DocumentTemplate { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies whether the new list is displayed on the Quick Launch of the site.
        /// </summary>
        [XmlAttribute]
        public bool OnQuickLaunch { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies the list server template of the new list.
        /// https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.listtemplatetype.aspx
        /// </summary>
        [XmlAttribute]
        public int TemplateType { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies whether the new list is displayed on the Quick Launch of the site.
        /// </summary>
        [XmlAttribute]
        public string Url { get; set; }
       
        /// <summary>
        /// Gets or sets whether verisioning is enabled on the list
        /// </summary>
        [XmlAttribute]
        public bool EnableVersioning { get; set; }

        /// <summary>
        /// Gets or sets whether to remove the default content type from the list
        /// </summary>
        [XmlAttribute]
        public bool RemoveDefaultContentType { get; set; }
  
        /// <summary>
        /// Gets or sets the content types to associate to the list
        /// </summary>
        [XmlArray(ElementName = "ContentTypeBindings")]
        [XmlArrayItem("ContentTypeBinding", typeof(ContentTypeBinding))]
        public List<ContentTypeBinding> GetContentTypeBindings
        {
            get { return this._ctBindings; }
            private set { this._ctBindings = value;}
        }
        #endregion

    }
}
