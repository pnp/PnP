using System;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public class Field : IEquatable<Field>
    {
        #region Private Members
        private string _schemaXml = string.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value that specifies the XML Schema representing the Field type.
        /// <seealso>
        ///     <cref>https://msdn.microsoft.com/en-us/library/office/ff407271.aspx</cref>
        /// </seealso>
        /// </summary>
        public string SchemaXml
        {
            get { return this._schemaXml; }
            set { this._schemaXml = value; }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            XElement element = PrepareFieldForCompare(this.SchemaXml);
            return element.ToString().GetHashCode();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Field))
            {
                return (false);
            }
            return (Equals((Field)obj));
        }

        public bool Equals(Field other)
        {
            XElement currentXml = PrepareFieldForCompare(this.SchemaXml);
            XElement otherXml = PrepareFieldForCompare(other.SchemaXml);
            return (XNode.DeepEquals(currentXml, otherXml));
        }

        private XElement PrepareFieldForCompare(string schemaXML)
        {
            XElement element = XElement.Parse(schemaXML);
            if (element.Attribute("SourceID") != null)
            {
                element.Attribute("SourceID").Remove();
            }

            return element;
        }
        #endregion
    }
}
