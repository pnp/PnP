using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.UserProfiles.Sync.XMLObjects
{
    [Serializable]
    [XmlRoot("Configuration")]
    public class SyncConfiguration
    {
        [XmlArray("Properties")]
        [XmlArrayItem("Property", typeof(Property))]
        public Property[] Properties;
    }
}
