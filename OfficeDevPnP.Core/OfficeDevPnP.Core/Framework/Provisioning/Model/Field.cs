using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;


namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML
    /// </summary>
    public class Field : IXmlSerializable
    {
        private string _schemaXml = string.Empty;

        /// <summary>
        /// Gets a value that specifies the XML Schema representing the Field type.
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXml; }
            set { this._schemaXml = value; }
        }

        /// <summary>
        /// No Imp will return null
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Field")
            {
                this._schemaXml = reader.ReadOuterXml();
            }
            //    reader.MoveToContent();

        }

        public void WriteXml(XmlWriter writer)
        {
            if (string.IsNullOrEmpty(this._schemaXml))
            {
                return;
            }
            XElement _fieldXML = XElement.Parse(this._schemaXml);
            foreach (var attrib in _fieldXML.Attributes())
            {
                writer.WriteAttributeString(attrib.Name.ToString(), attrib.Value);
            }
        }
    }
}
