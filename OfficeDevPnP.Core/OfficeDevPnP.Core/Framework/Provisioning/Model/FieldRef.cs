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
    public class FieldRef : BaseModelEntity
    {
        #region Private Members

        private Guid _ID = Guid.Empty;
        #endregion

        #region Public Properties
        /// <summary>
        /// Gets a value that specifies the XML Schema representing the Field type.
        /// <seealso cref="https://msdn.microsoft.com/en-us/library/office/ff407271.aspx"/>
        /// </summary>
        public Guid ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        #endregion

        #region Comparison code

        public override int CompareTo(Object obj)
        {
            FieldRef other = obj as FieldRef;

            if (other == null)
            {
                return (1);
            }

            if (this.ID == other.ID)
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
                this.ID).GetHashCode());
        }

        #endregion
    }
}
