using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public class FieldRef : IEquatable<FieldRef>
    {
        #region Private Members

        private Guid _ID = Guid.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets a value that specifies the XML Schema representing the Field type.
        /// <seealso>
        ///     <cref>https://msdn.microsoft.com/en-us/library/office/ff407271.aspx</cref>
        /// </seealso>
        /// </summary>
        public Guid ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}",
                this.ID).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is FieldRef))
            {
                return (false);
            }
            return (Equals((FieldRef)obj));
        }

        public bool Equals(FieldRef other)
        {
            return (this.ID == other.ID);
        }

        #endregion
    }
}
