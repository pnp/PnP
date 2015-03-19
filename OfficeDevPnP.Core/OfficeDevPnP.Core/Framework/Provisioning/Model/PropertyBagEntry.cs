using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning
{
    public class PropertyBagEntry
    {
        [XmlAttribute]
        public string Key { get; set; }
            
        [XmlAttribute]
        public string Value { get; set; }
    }
}
