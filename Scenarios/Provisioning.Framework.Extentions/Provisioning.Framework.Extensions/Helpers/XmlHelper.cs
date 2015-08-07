using OfficeDevPnP.Core.Utilities;
using System.IO;
using System.Xml.Linq;

namespace Provisioning.Framework.Extensions
{
    class XmlHelper
    {
        public static T ReadXml<T>(string filename)
        where T : class
        {
            T res = default(T);
            using (StreamReader reader = new StreamReader(filename))
            {
                XDocument doc = XDocument.Load(reader);
                res = XMLSerializer.Deserialize<T>(doc);
            }
            return res;
        }

        public static T ReadXmlString<T>(string xml)
            where T : new()
        {
            T res = default(T);
            XDocument doc = XDocument.Parse(xml);
            res = XMLSerializer.Deserialize<T>(doc);
            return res;
        }

        public static void WriteXml(object obj, string filename)
        {
            string xml = XMLSerializer.Serialize(obj);
            using (StreamWriter writer = new StreamWriter(filename))
            {
                writer.Write(xml);
                writer.Close();
            }
        }
    }
}
