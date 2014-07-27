using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml.Serialization;
using System.IO;
using System.Configuration;

namespace Contoso.ProfileProperty.Migration.Extract
{
    [Serializable()]
    public class UserProfileCollection
    {

        public List<UserProfileData> ProfileData;
        
        public void Save()
        {
            XmlSerializer x = new XmlSerializer(typeof(UserProfileCollection));
            TextWriter writer = new StreamWriter(ConfigurationManager.AppSettings["USERPROFILESSTORE"]);
            x.Serialize(writer, this);
            writer.Close();

        }
    }
}
