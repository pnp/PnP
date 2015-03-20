using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class Feature
    {     
        [XmlAttribute]
        public string ID { get; set; }
        [XmlAttribute]
        public bool Deactivate { get; set; }
    }
}
