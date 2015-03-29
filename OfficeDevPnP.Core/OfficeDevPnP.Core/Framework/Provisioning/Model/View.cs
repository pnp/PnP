using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    public class View : BaseModelEntity
    {
        #region Private Members
        private string _schemaXml = string.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value that specifies the XML Schema representing the View type.
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
            View other = obj as View;

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
