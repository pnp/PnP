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
            XElement element = XElement.Parse(this.SchemaXml);
            if (element.Attribute("SourceID") != null)
            {
                element.Attribute("SourceID").Remove();
            }
            //return (String.Format("{0}",
                //this.SchemaXml).GetHashCode()); 
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
            XElement currentXml = XElement.Parse(this.SchemaXml);
            XElement otherXml = XElement.Parse(other.SchemaXml);

            if (currentXml.Attribute("SourceID") != null)
            {
                currentXml.Attribute("SourceID").Remove();
            }
            if(otherXml.Attribute("SourceID") != null)
            {
                otherXml.Attribute("SourceID").Remove();
            }
            return (XNode.DeepEquals(currentXml, otherXml));
        }

        #endregion
    }
}
