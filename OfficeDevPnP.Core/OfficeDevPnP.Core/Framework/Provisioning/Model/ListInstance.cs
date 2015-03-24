using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that specifies the properties of the new list.
    /// </summary>
    public class ListInstance
    {
        #region Constructors

        public ListInstance() { }

        public ListInstance(IEnumerable<ContentTypeBinding> contentTypeBindings,
            IEnumerable<View> views)
        {
            if (contentTypeBindings != null)
            {
                this.ContentTypeBindings.AddRange(contentTypeBindings);
            }

            if (views != null)
            {
                this.Views.AddRange(views);
            }
        }

        #endregion

        #region Private Members
        private List<ContentTypeBinding> _ctBindings = new List<ContentTypeBinding>();
        private List<View> _views = new List<View>();
        #endregion

        #region Properties
        /// <summary>
        /// Gets or sets the list title
        /// </summary>
        public string Title { get; set; }
        
        /// <summary>
        /// Gets or sets the description of the list
        /// </summary>
        public string Description { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies the identifier of the document template for the new list.
        /// </summary>
        public string DocumentTemplate { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies whether the new list is displayed on the Quick Launch of the site.
        /// </summary>
        public bool OnQuickLaunch { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies the list server template of the new list.
        /// https://msdn.microsoft.com/en-us/library/office/microsoft.sharepoint.client.listtemplatetype.aspx
        /// </summary>
        public int TemplateType { get; set; }
        
        /// <summary>
        /// Gets or sets a value that specifies whether the new list is displayed on the Quick Launch of the site.
        /// </summary>
        public string Url { get; set; }
       
        /// <summary>
        /// Gets or sets whether verisioning is enabled on the list
        /// </summary>
        public bool EnableVersioning { get; set; }

        /// <summary>
        /// Gets or sets the MinorVersionLimit  for verisioning, just in case it is enabled on the list
        /// </summary>
        public int MinorVersionLimit { get; set; }

        /// <summary>
        /// Gets or sets the MinorVersionLimit  for verisioning, just in case it is enabled on the list
        /// </summary>
        public int MaxVersionLimit { get; set; }

        /// <summary>
        /// Gets or sets whether to remove the default content type from the list
        /// </summary>
        public bool RemoveDefaultContentType { get; set; }
  
        /// <summary>
        /// Gets or sets whether content types are enabled
        /// </summary>
        public bool ContentTypesEnabled { get; set; }

        /// <summary>
        /// Gets or sets whether to hide the list
        /// </summary>
        public bool Hidden { get; set; }

        /// <summary>
        /// Gets or sets the content types to associate to the list
        /// </summary>
        public List<ContentTypeBinding> ContentTypeBindings
        {
            get { return this._ctBindings; }
            private set { this._ctBindings = value;}
        }

        /// <summary>
        /// Gets or sets the content types to associate to the list
        /// </summary>
        public List<View> Views
        {
            get { return this._views; }
            private set { this._views = value; }
        }
        #endregion

    }
}
