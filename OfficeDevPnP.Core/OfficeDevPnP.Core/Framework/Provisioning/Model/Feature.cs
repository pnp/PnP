using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that represents a Feature
    /// </summary>
    public class Feature
    {
        [XmlAttribute]
        public Guid ID { get; set; }

        [XmlAttribute]
        public bool Deactivate { get; set; }
    }
}
