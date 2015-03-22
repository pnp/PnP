using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object for Content Type Binding in the Provisioning Template 
    /// </summary>
    public class ContentTypeBinding
    {
        #region Properties
        /// <summary>
        /// Gets or Sets the Content Type ID 
        /// </summary>
        [XmlAttribute]
        public string ContentTypeID { get; set; }
        
        /// <summary>
        /// Gets or Sets if the Content Type should be the default Content Type in the library
        /// </summary>
        [XmlAttribute]
        public bool Default { get; set; }
        #endregion

    }
}
