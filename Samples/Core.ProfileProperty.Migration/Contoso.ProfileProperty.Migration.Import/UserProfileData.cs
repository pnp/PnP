using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;

namespace Contoso.ProfileProperty.Migration.Import
{
    [Serializable()]
    public class UserProfileData
    {

        [XmlElement]
        public string UserName;

        [XmlElement]
        public string AboutMe;

        [XmlElement]
        public string AskMeAbout;
    }
}
