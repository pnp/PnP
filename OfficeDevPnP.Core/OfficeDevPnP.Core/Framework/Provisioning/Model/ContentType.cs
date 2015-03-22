using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;
using System.Xml.Serialization;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object used in the Provisioning template that defines a Content Type
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ms463449.aspx"/>
    /// </summary>
    public class ContentType : IXmlSerializable
    {
        #region Private Members
        private string _schemaXML = string.Empty;
        #endregion

        #region Properties
        /// <summary>
        /// Gets or Sets value that specifies the XML Schema representing the content type.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ms463449.aspx"/>
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXML; }
            set { this._schemaXML = value; }
        }
        #endregion

        #region IXmlSerializable Members
        /// <summary>
        /// No Implementation will return null
        /// </summary>
        /// <returns></returns>
        public XmlSchema GetSchema()
        {
            return null;
        }

        /// <summary>
        /// Generates an object from its XML representation.
        /// </summary>
        /// <param name="reader"></param>
        public void ReadXml(System.Xml.XmlReader reader)
        {
            if (reader.MoveToContent() == System.Xml.XmlNodeType.Element && reader.LocalName == "ContentType")
           {
               this._schemaXML = reader.ReadOuterXml();
           }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
        public void WriteXml(System.Xml.XmlWriter writer)
        {
            if (string.IsNullOrEmpty(this._schemaXML)) return;
            XElement _xmlElement = XElement.Parse(this._schemaXML);
            foreach(var attrib in _xmlElement.Attributes())
            {
                writer.WriteAttributeString(attrib.Name.ToString(), attrib.Value);
            }
     
            foreach(var _element in _xmlElement.Elements())
            {
                writer.WriteStartElement(_element.Name.ToString(), _element.Value);
            
                foreach(var element in _element.Elements())
                {
                    writer.WriteRaw(element.ToString());
                }
                writer.WriteEndElement();
            }

        }

        #endregion
    }
}
