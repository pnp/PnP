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
        /// Gets ot sets the ID of the referenced field
        /// </summary>
        public Guid ID
        {
            get { return this._ID; }
            set { this._ID = value; }
        }

        public string DisplayName { get; set; }

        /// <summary>
        /// Gets or sets if the field is Required
        /// </summary>
        public bool Required { get; set; }

        /// <summary>
        /// Gets or sets if the field is Hidden
        /// </summary>
        public bool Hidden { get; set; }
        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.ID,
                this.Required,
                this.Hidden).GetHashCode());
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
            return (this.ID == other.ID &&
                this.Required == other.Required &&
                this.Hidden == other.Hidden);
        }

        #endregion
    }
}
