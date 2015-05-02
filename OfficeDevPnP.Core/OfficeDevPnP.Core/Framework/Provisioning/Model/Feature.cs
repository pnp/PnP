using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that represents an Feature.
    /// </summary>
    public partial class Feature : IEquatable<Feature>
    {
        #region Private Members

        private Guid _id;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the feature Id
        /// </summary>
        public Guid Id { get { return _id; } set { _id = value; } }

        /// <summary>
        /// Gets or sets if the feature should be deactivated
        /// </summary>
        public bool Deactivate { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}|{1}",
                this.Deactivate,
                this.Id).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is Feature))
            {
                return (false);
            }
            return (Equals((Feature)obj));
        }

        public bool Equals(Feature other)
        {
            return (this.Deactivate == other.Deactivate &&
                this.Id == other.Id);
        }

        #endregion
    }
}
