using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Linq;
using System.Xml.Schema;


namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public class Field : BaseModelEntity
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

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            Field other = obj as Field;

            if (other == null)
            {
                return (1);
            }

            XElement currentXml = XElement.Parse(this.SchemaXml);
            XElement otherXml = XElement.Parse(other.SchemaXml);

            if (XNode.DeepEquals(currentXml, otherXml))
            {
                return (0);
            }
            else
            {
                return (-1);
            }
        }

        public override int GetHashCode()
        {
            return (String.Format("{0}",
                this.SchemaXml).GetHashCode()); 
        }

        #endregion
    }
}
