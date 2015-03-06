using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Framework.Provisioning.Core.Configuration.Template
{
    /// <summary>
    /// Domain Object used in the site template that defines a Content Type
    /// </summary>
    public class ContentType
    {
        /// <summary>
        /// Gets a value that specifies the XML Schema representing the content type.
        /// </summary>
        public string SchemaXml
        {
            get;
            set;
        }
    }
}
