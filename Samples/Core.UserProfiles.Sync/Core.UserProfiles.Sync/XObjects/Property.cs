using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace Core.UserProfiles.Sync.XMLObjects
{
    [Serializable]
    public class Property
    {
        [XmlAttribute("ADAttributeName")]
        public string ADAttributeName;

        [XmlAttribute("UserProfileAttributeName")]
        public string UserProfileAttributeName;

        [XmlAttribute("IsExtendedProperty")]
        public bool IsExtended;

        [XmlAttribute("WriteIfBlank")]
        public bool WriteIfBlank;

        [XmlAttribute("IsMulti")]
        public bool IsMulti;
    }
}
