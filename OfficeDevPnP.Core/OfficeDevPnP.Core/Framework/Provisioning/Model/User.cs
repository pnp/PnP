using System;

namespace OfficeDevPnP.Core.Framework.Provisioning.Model
{
    /// <summary>
    /// Domain Object that defines a User or group in the provisioning template
    /// </summary>
    public partial class User : IEquatable<User>
    {
        #region Properties

        /// <summary>
        /// The User email Address or the group name.
        /// </summary>
        public string Name { get; set; }

        #endregion

        #region Comparison code

        public override int GetHashCode()
        {
            return (String.Format("{0}",
                this.Name).GetHashCode());
        }

        public override bool Equals(object obj)
        {
            if (!(obj is User))
            {
                return (false);
            }
            return (Equals((User)obj));
        }

        public bool Equals(User other)
        {
            return (this.Name == other.Name);
        }

        #endregion
    }
}
