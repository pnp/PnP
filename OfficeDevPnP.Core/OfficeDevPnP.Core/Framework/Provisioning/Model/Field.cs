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
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public class Field : IXmlSerializable
    {
        #region Private Members
        private string _schemaXml = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value that specifies the XML Schema representing the Field type.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ff407271.aspx"/>
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXml; }
            set { this._schemaXml = value; }
        }

        #endregion

        #region IXmlSerializable
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
        public void ReadXml(XmlReader reader)
        {
            if (reader.MoveToContent() == XmlNodeType.Element && reader.LocalName == "Field")
            {
                this._schemaXml = reader.ReadOuterXml();
            }
        }

        /// <summary>
        /// Converts an object into its XML representation.
        /// </summary>
        /// <param name="writer"></param>
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
        #endregion
    }
}
