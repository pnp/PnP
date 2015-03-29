using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Schema;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object used in the Provisioning template that defines a Content Type
    /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ms463449.aspx"/>
    /// </summary>
    public class ContentType : BaseModelEntity
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

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            ContentType other = obj as ContentType;

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
