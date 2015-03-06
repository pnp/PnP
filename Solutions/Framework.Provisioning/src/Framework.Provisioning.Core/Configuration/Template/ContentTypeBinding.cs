using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// Domain Object for Content Type Binding in the site tempalte
    /// </summary>
    public class ContentTypeBinding
    {
        /// <summary>
        /// Content Type ID
        /// </summary>
        [XmlAttribute]
        public string ContentTypeID { get; set; }
        /// <summary>
        /// Gets if the Content Type should be the default Content Type in the library
        /// </summary>
        [XmlAttribute]
        public bool Default { get; set; }
    }
}
