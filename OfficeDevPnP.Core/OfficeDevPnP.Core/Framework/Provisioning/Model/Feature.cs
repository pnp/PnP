using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that represents an Feature.
    /// </summary>
    public class Feature
    {
        /// <summary>
        /// Gets or sets the feature ID
        /// </summary>
        [XmlAttribute]
        public Guid ID { get; set; }

        /// <summary>
        /// Gets or sets if the feature should be deactivated
        /// </summary>
        [XmlAttribute]
        public bool Deactivate { get; set; }
    }
}
