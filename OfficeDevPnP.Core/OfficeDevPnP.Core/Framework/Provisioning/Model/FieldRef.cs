using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Represents a Field XML Markup that is used to define information about a field
    /// </summary>
    public partial class FieldRef : IEquatable<FieldRef>
    {
        #region Private Members

        private Guid _id = Guid.Empty;
        #endregion

        #region Public Properties

        /// <summary>
        /// Gets ot sets the ID of the referenced field
        /// </summary>
        public Guid Id
        {
            get { return this._id; }
            set { this._id = value; }
        }

        /// <summary>
        /// Gets or sets the name of the field link. This will not change the internal name of the field.
        /// </summary>
        public string Name { get; private set; }
        
        /// <summary>
        /// Gets or sets the Display Name of the field. Only applicable to fields associated with lists.
        /// </summary>
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

        #region Constructors

        public FieldRef()
        {
        }

        public FieldRef(string fieldRefName)
        {
            this.Name = fieldRefName;
        }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}|{2}",
                this.Id,
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
            return (this.Id == other.Id &&
                this.Required == other.Required &&
                this.Hidden == other.Hidden);
        }

        #endregion
    }
}
