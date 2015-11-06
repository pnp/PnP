using Perficient.Provisioning.VSTools.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace Perficient.Provisioning.VSTools.Helpers
{
    public static class XmlHelpers
    {
        public static XElement GetElementByLocalName(XElement parent, string name)
        {
            return parent.Elements().Where(e => e.Name.LocalName == name).FirstOrDefault();
        }

        public static ProvisioningTemplateToolsConfiguration DeserializeObject(string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProvisioningTemplateToolsConfiguration));
            FileStream fs = new FileStream(filename, FileMode.Open);
            XmlReader reader = XmlReader.Create(fs);

            ProvisioningTemplateToolsConfiguration config;
            config = (ProvisioningTemplateToolsConfiguration)serializer.Deserialize(reader);
            fs.Close();

            return config;
        }

        public static void SerializeObject(ProvisioningTemplateToolsConfiguration config, string filename)
        {
            XmlSerializer serializer = new XmlSerializer(typeof(ProvisioningTemplateToolsConfiguration));
            using (MemoryStream ms = new MemoryStream())
            {
                XmlWriterSettings settings = new XmlWriterSettings();
                settings.Indent = true;
                settings.NewLineOnAttributes = true;
                XmlWriter writer = XmlWriter.Create(ms, settings);
                serializer.Serialize(writer, config);

                System.IO.File.WriteAllBytes(filename, ms.ToArray());
            }

        } 
    }
}
